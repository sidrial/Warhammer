/// <reference path="typings/jquery.d.ts" />
;
$(function () {
    var table = $("#matches").DataTable({
        "processing": false,
        "serverSide": true,
        "orderMulti": false,
        "targets": 'no-sort',
        "bFilter": false,
        "bSort": false,
        "lengthChange": false,
        "order": [],
        "pageLength": 15,
        "ajax": {
            "url": "getmatches",
            "type": "POST",
            "datatype": "json"
        },       
        "columns": [
            { "data": "creationDateTime", "name": "Datum", "autoWidth": true },
            { "data": "winner", "name": "Winnaar", "autoWidth": true },
            { "data": "loser", "name": "Verliezer", "autoWidth": true },
            { "data": "rating", "name": "Rating", "autoWidth": true },
            { "data": "status", "name": "Status", "autoWidth": true },
            { "data": "confirm", "name": "Confirm", "autoWidth": true }
        ]
    });
    $('#matches tbody').on('click', 'button', function () {
        var data = table.row($(this).parents('tr')).data();
        var post = $.post(`confirm `, { matchId: data.matchId });
        post.done(() => { window.location.href = "overview"; });
    });
});