$(document).ready(function () {
    var accounts = []
    var users = []

    var idIncremental



    function GetAccounts() {
        $.ajax({
            url: urlGetAccounts,
            type: "POST",
            success: function (result) {
                if (result != "") {
                    accounts = result
                    CreateTable(accounts)
                }
            },
            error: function (error) {
                console.log("error")
                console.log(error)
            }
        })
    }

    function CreateTable(accounts) {
        const element = document.getElementById('tbody')
        if (element != null)
            element.remove()

        var rows = ""
        accounts.forEach(function (item) {
            var users = ""
            item.Users.forEach(function (key) {
                if (JSON.stringify(item.Users.slice(-1)) === JSON.stringify(Object.is(key)))
                    users += key.Name
                else
                    users += key.Name + " - "
            })
            rows += "<tr><td>" + item.Name + "</td><td>" + item.UserExpenses + "</td><td>" + item.TotalExpenses + "</td><td>" + users
                + '</td><td><input type="button" class="btn btn-outline-danger btnDeleteAccounts" id="btnDelete_' + item.Id.Increment + '" value="X"/>'
                + '</td><td><input type="button" class="btn btn-outline-primary btnInfoAccounts" id="btnInfo_' + item.Id.Increment + '" value="!"/>'
                + '</td><td><input type="button" class="btn btn-outline-primary btnAccounts" id="btn_' + item.Id.Increment + '" value=">"/></td></tr>'
        })

        $('#tableAccounts').append('<tbody id="tbody">' + rows + '</tbody>')
    }

    $(document).on('keyup', '#txtFilter', function () {
        var val = $('#txtFilter').val()
        if (val != "") {
            $.ajax({
                url: urlGetUsers,
                data: { filter: val },
                type: "POST",
                success: function (result) {
                    if (result != "") {
                        CreateSelect(result)
                    }
                },
                error: function (error) {
                    console.log("error")
                    console.log(error)
                }
            })
        }
    })

    function CreateSelect(users) {
        $('#selectSearch').empty();
        var option = ""
        users.forEach(function (key) {
            option += '<option value"' + key + '">' + key + '</option>'
        })

        $('#selectSearch').append(option)
    }

    $(document).click(function (e) {
        if (e.target.id == "btnAddUser") {
            var val = $('#selectSearch').val()
            if (val != "" && !users.find(x => x == val)) {
                users.push(val)

                var html = '<div class="col-6" id="divUser_' + val + '"><div class="row"><div class="col-10"><p>' + val + '</p></div><div class="col-2"><input type="button" class="btn btn-outline-danger btnDeleteUsers" id="btnDeleteUser_' + val + '" value="x"></div></div></div>'
                $('#divUsers').append(html)
            }
        }
    })

    $(document).on('click', '#btnAddAccount', function (e) {
        var name = $('#txtName').val()
        console.log(users)
        if (name != "") {
            $.ajax({
                url: urlInsertAccount,
                data: { name: name, users: users },
                type: "POST",
                success: function (result) {
                    if (result != "") {
                        accounts = result
                        CreateTable(accounts)

                        $('#btnCloseModal').click()

                        $('#txtName').value = ""
                        users = []
                    }
                },
                error: function (error) {
                    console.log("error")
                    console.log(error)
                }
            })
        }
        else
            alert('Insert the name of new account')
    })

    $(document).on('click', '#btnDeleteAccount', function () {
        console.log(idIncremental)
        $.ajax({
            url: urlDeleteAccount,
            data: { idIncremental: idIncremental },
            type: "POST",
            success: function (result) {
                if (result) {
                    $('#modalDeleteAccount').modal('hide')

                    GetAccounts()
                }
            },
            error: function (error) {
                console.log("error")
                console.log(error)
            }
        })
    })
    
    function GetExpenses() {
        $.ajax({
            url: urlGetExpense,
            type: "POST",
            success: function (result) {
                if (result != "") {
                    console.log(result)
                    CreateTable(result)
                }
            },
            error: function (error) {
                console.log("error")
                console.log(error)
            }
        })
    }

    function CreateTable(expenses) {
        const element = document.getElementById('tbody')
        if (element != null)
            element.remove()

        var rows = ""
        expenses.forEach(function (item) {
            var d = item.Date.substring(9, 13)
            var date = new Date(d).toLocaleDateString('en-GB')
            rows += "<tr><td>" + item.Name + "</td><td>" + item.PaidBy + "</td><td>" + date + "</td><td>" + item.Cost
                + '</td><td><div class="row"><div class="col"><input type="button" class="btn btn-outline-danger btnDeleteExpenses" id="btnDelete_' + item.Id.Increment + '" value="Delete"/></div>'
                + '<div class="col"><input type="button" class="btn btn-outline-primary btnInfoExpenses" id="btnInfo_' + item.Id.Increment + '" value="Info"/></div>'
        })

        $('#tableExpenses').append('<tbody id="tbody">' + rows + '</tbody>')
    }

    document.addEventListener('click', function (e) {
        if (e.target.className.split(' ')[2] == "btnAccounts") {
            idIncremental = e.target.id.substring(4)
            $.ajax({
                url: urlAccount,
                data: { idIncremental: idIncremental },
                type: "POST",
                success: function (result) {
                    if (result != "") {
                        $('#divLoadComponent').empty()

                        $('#divLoadComponent').html(result)

                        GetExpenses()
                    }
                },
                error: function (error) {
                    console.log("error")
                    console.log(error)
                }
            })
        }
        else if (e.target.className.split(' ')[2] == "btnDeleteAccounts") {
            idIncremental = e.target.id.substring(10)

            $('#modalDeleteAccount').modal('show')
        }
        else if (e.target.className.split(' ').find(x => x == "btnDeleteUsers")) {
            var username = e.target.id.substring(14)
            console.log(username)
            $('#divUser_' + username).remove()

            var index = users.indexOf(username)
            users.splice(index, 1)
        }
    })
})