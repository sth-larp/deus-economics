var slave_button = '<input class="slave_button" id="slave_btn{0}" \
type="button" value="{1} ({2})" onclick="setCurrentSlave(this);" />';

var ROLE = {
    NONE: { value: 0, name: "None"},
    READ: { value: 1, name: "Read"},
    WITHDRAW: { value: 2, name: "Withdraw" },
    ADMIN: { value: 4, name: "Admin" },
    MASTER: { value: 8, name: "Master" }
};

function get_role(role) {
    for (var i in ROLE) {
        if (ROLE[i].name == role)
            return ROLE[i];
    }
    return ROLE[0];
}

function format(source, params) {
    $.each(params, function (i, n) {
        source = source.replace(new RegExp("\\{" + i + "\\}", "g"), n);
    })
    return source;
}

function setCurrentSlave(item) {
    if (!$('#' + item.id).hasClass('selected_slave'))
    {
        $('.selected_slave').removeClass('selected_slave');
        $('#' + item.id).addClass('selected_slave');

        var role = get_role(item.dataset.role);
        setCurrentAccount(item.dataset.acc, role);
    }
}

function add_slave_row(acc, role, id, table) {
    var row = table.insertRow(0);
    var cell = row.insertCell(0);

    cell.innerHTML = format(slave_button, [id, acc, role]);

    var elem = document.getElementById('slave_btn' + id);
    elem.dataset.acc = acc;
    elem.dataset.role = role;
}

function fill_slaves(json, master) {
    var table = document.getElementById("slave_table");
    if (json.length > 0)
    {
        var accesses = JSON.parse(json);

        for (var i in accesses) {
            var acc = accesses[i];
            add_slave_row(acc.Slave, acc.Role, i, table);
        }
    }
    add_slave_row(master, ROLE.MASTER.name, 333, table);
}