var slave_elem = '<div class="slave-menu-elem" id="slave_btn{0}" \
    onclick="setCurrentSlave(this);">{1} ({2})</div>';

function setCurrentSlave(item) {
    showLeftMenu(false);

    if (!$('#' + item.id).hasClass('selected-slave'))
    {
        $('.selected-slave').removeClass('selected-slave');
        $('#' + item.id).addClass('selected-slave');

        var role = get_role(item.dataset.role);
        setCurrentAccount(item.dataset.acc, role);
    }
}

function add_slave_row(acc, roleName, id, box) {
    var role = get_role(roleName);
    var newbox = format(slave_elem, [id, acc, role.rusname]);
    box.innerHTML += newbox;

    var elem = document.getElementById('slave_btn' + id);
    elem.dataset.acc = acc;
    elem.dataset.role = role.name;
}

function fill_slave_menu(json, master) {
    var slavebox = document.getElementById("slave_list");
    add_slave_row(master, ROLE.MASTER.name, 333, slavebox);
    $('#slave_btn333').addClass('selected-slave');

    if (json.length > 0)
    {
        var accesses = JSON.parse(json);

        for (var i in accesses) {
            var acc = accesses[i];
            add_slave_row(acc.Slave, acc.Role, i, slavebox);
        }
    }
}