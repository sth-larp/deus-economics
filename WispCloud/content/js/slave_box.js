var slave_button = '<input class="slave_button" id="slave_btn{0}" \
type="button" value="{1} ({2})" onclick="setCurrentSlave(this);" />';

function setCurrentSlave(item) {
    if (!$('#' + item.id).hasClass('selected_slave'))
    {
        $('.selected_slave').removeClass('selected_slave');
        $('#' + item.id).addClass('selected_slave');

        var role = get_role(item.dataset.role);
        setCurrentAccount(item.dataset.acc, role);
    }
}

function add_slave_row(acc, roleName, id, table) {
    var row = table.insertRow(0);
    var cell = row.insertCell(0);

    var role = get_role(roleName);

    cell.innerHTML = format(slave_button, [id, acc, role.rusname]);

    var elem = document.getElementById('slave_btn' + id);
    elem.dataset.acc = acc;
    elem.dataset.role = role.name;
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
    $('#slave_btn333').addClass('selected_slave');
}