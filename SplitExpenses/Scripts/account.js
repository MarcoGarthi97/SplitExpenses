$(document).ready(function () {
    var users = []
    var usersFor = []

    var idIncremental = 0

    function GetExpenses() {
        $.ajax({
            url: urlGetExpenses,
            type: "POST",
            success: function (result) {
                if (result != "") {
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
            var d = parseInt(item.Date.substring(6, 19))
            var date = new Date(d).toLocaleDateString('en-GB')

            rows += "<tr><td>" + item.Name + "</td><td>" + item.PaidBy + "</td><td>" + date + "</td><td>" + item.Cost
                + '</td><td><div class="row"><div class="col"><input type="button" class="btn btn-outline-danger btnDeleteExpenses" id="btnDelete_' + item.Id.Increment + '" value="X"/></div>'
                + '<div class="col"><input type="button" class="btn btn-outline-primary btnInfoExpenses" id="btnInfo_' + item.Id.Increment + '" value="?"/></div>'
        })

        $('#tableExpenses').append('<tbody id="tbody">' + rows + '</tbody>')
    }

    function GetAccount() {
        $.ajax({
            url: urlGetAccount,
            type: "POST",
            success: function (result) {
                if (result != "") {
                    console.log(result)
                    $('#pNameAccount').text(result.Name)
                    users = []

                    result.Users.forEach(function (key) {
                        users.push(key)
                    })

                    InsertOptionToSelectPaid()
                }
            },
            error: function (error) {
                console.log("error")
                console.log(error)
            }
        })
    }

    function GetBalance() {
        $.ajax({
            url: urlGetBalance,
            type: "POST",
            success: function (result) {
                if (result != "") {
                    CreateBalance(result)
                }
            },
            error: function (error) {
                console.log("error")
                console.log(error)
            }
        })
    }

    function CreateBalance(balance) {
        $("#divBalance").remove()

        var positive = 0
        var negative = 0

        balance.forEach(function (key) {
            if (key.Balance > 0)
                positive += key.Balance
            else
                negative += key.Balance
        })

        var list = []
        var perc

        balance.forEach(function (key) {
            if (key.Balance > 0)
                perc = key.Balance * 100 / positive
            else
                perc = key.Balance * 100 / negative

            key.Perc = perc.toFixed(2)

            list.push(key)
        })

        list.sort((a, b) => b.Balance - a.Balance)

        var row = ""
        var classCss = ""
        list.forEach(function (key) {
            if (key.Balance > 0)
                classCss = 'colored-bar-green'
            else
                classCss = 'colored-bar-red'

            var b = key.Balance.toFixed(2)

            row += '<div class="row"><div class="col-1"><p>' + key.Name + '</p></div><div class="col-9"><div class="colored-bar ' + classCss + '" style="width: ' + key.Perc + '%;"></div></div><div class="col-2"><p>Balance: ' + b + '</p></div></div>'
        })

        row = '<div id="divBalance">' + row + '<div>'
        $('#footer').append(row)
    }

    function InsertOptionToSelectPaid() {
        $('#selectPaid').empty()

        var option = '<option value""></option>'
        users.forEach(function (key) {
            option += '<option value"' + key.Name + '">' + key.Name + '</option>'
        })

        $('#selectPaid').append(option)
    }

    $(document).on('change', '#selectPaid', function () {
        LoadSelectPaid()
    })

    function LoadSelectPaid(){
        var val = $('#selectPaid').val()
        if (val != '') {
            usersFor = []
            users.forEach(function (key) {
                if (key.Name !== val)
                    usersFor.push(key)
            })
            InsertOptionToSelectFor(usersFor)
        }
    }

    function InsertOptionToSelectFor(usersFor) {
        $('#selectFor').empty()
        $('#divUsers').empty()

        var option = '<option value""></option>'
        var html = ''
        usersFor.forEach(function (key) {
            option += '<option value"' + key.Name + '">' + key.Name + '</option>'
            html += '<div class="col-6" id="divUser_' + key.Name + '"><div class="row"><div class="col-10"><p>' + key.Name + '</p></div><div class="col-2"><input type="button" class="btn btn-outline-primary btnDeleteUsers" id="btnDeleteUser_' + key + '" value="x"></div></div></div>'
        })

        $('#selectFor').append(option)
        $('#divUsers').append(html)
    }

    $(document).click(function (e) {
        if (e.target.id == "btnAddUserSelect") {
            var val = $('#selectSearch').val()
            if (val != "" && !users.find(x => x == val)) {
                users.push(val)

                var html = '<div class="col-6" id="divUser_' + val + '"><div class="row"><div class="col-10"><p>' + val + '</p></div><div class="col-2"><input type="button" class="btn btn-outline-danger btnDeleteUsers" id="btnDeleteUser_' + val + '" value="x"></div></div></div>'
                $('#divUsers').append(html)
            }
        }
    })

    $(document).on('click', '#btnAddExpense', function (e) {
        var name = $('#txtName').val()
        var cost = $('#numberCost').val()
        var d = $('#date').val()
        var owner = $('#selectPaid').val()

        var listUsers = []
        usersFor.forEach(function (key) {
            listUsers.push(key.Name)
        })

        if (name != '' && cost != '' && cost != '0' && d != '') {
            if (idIncremental == 0) {
                $.ajax({
                    url: urlInsertExpense,
                    data: { name: name, cost: cost, date: d, owner: owner, usersFor: listUsers },
                    type: "POST",
                    success: function (result) {
                        if (result) {
                            $('#btnCloseModal').click()
                            GetExpenses()
                        }
                    },
                    error: function (error) {
                        console.log("error")
                        console.log(error.responseText)
                    }
                })
            }
            else {
                $.ajax({
                    url: urlUpdateExpense,
                    data: { idIncremental: idIncremental, name: name, cost: cost, date: d, owner: owner, usersFor: listUsers },
                    type: "POST",
                    success: function (result) {
                        if (result) {
                            $('#btnCloseModal').click()
                            GetExpenses()

                            $('#txtName').val('')
                            $('#numberCost').val('')
                            $('#date').val('')
                            $('#selectPaid').val('')
                        }
                    },
                    error: function (error) {
                        console.log("error")
                        console.log(error.responseText)
                    }
                })
            }
        }
    })

    $(document).on('click', '#btnModalExpense', function (e) {
        GetAccount()

        var now = new Date();

        var day = ("0" + now.getDate()).slice(-2);
        var month = ("0" + (now.getMonth() + 1)).slice(-2);

        var today = now.getFullYear() + "-" + (month) + "-" + (day);
        $('#date').val(today)
    })

    document.addEventListener('click', function (e) {
        if (e.target.className.split(' ').find(x => x == "btnDeleteUsers")) {
            var username = e.target.id.substring(14)
            $('#divUser_' + username).remove()

            var index = users.indexOf(username)
            users.splice(index, 1)
        }
        else if (e.target.className.split(' ').find(x => x == 'btnDeleteExpenses')) {
            if (confirm("Are you sure delete the account?") == true) {
                idIncremental = e.target.id.substring(10)
                $.ajax({
                    url: urlDeleteExpense,
                    data: { idIncremental: idIncremental },
                    type: "POST",
                    success: function (result) {
                        if (result) {
                            GetExpenses()
                        }

                        idIncremental = 0
                    },
                    error: function (error) {
                        console.log("error")
                        console.log(error)

                        idIncremental = 0
                    }
                })
            }
        }
        else if (e.target.className.split(' ').find(x => x == 'btnInfoExpenses')) {
            idIncremental = e.target.id.substring(8)

            $.ajax({
                url: urlGetExpense,
                data: { idIncremental: idIncremental },
                type: "POST",
                success: function (result) {
                    var expense = result
                    if (result = ! "") {
                        console.log(expense)
                        $('#txtName').val(expense.Name)
                        $('#numberCost').val(expense.Cost)

                        var username = expense.PaidBy.toLowerCase()
                        console.log(username)
                        $("#selectPaid option[value='" + username + "']").prop("selected", true);
                        //$('#selectPaid').val(username)

                        //LoadSelectPaid()

                        var d = parseInt(expense.Date.substring(6, 19))
                        var date = new Date(d).toISOString().slice(0, 10)
                        $('#date').val(date)

                        $('#modalAddExpense').modal('show')
                    }
                },
                error: function (error) {
                    console.log("error")
                    console.log(error)
                }
            })
        }
        else if (e.target.className.split(' ')[2] == "btnAccounts") {
            const timer1 = setTimeout(GetAccount, 1000);
            const timer2 = setTimeout(GetBalance, 1000);
        }
    })
})