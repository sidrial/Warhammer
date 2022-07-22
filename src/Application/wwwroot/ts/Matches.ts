/// <reference path="typings/jquery.d.ts" />

interface JQuery {
    DataTable: any
};

$(() => {
    var table = $("#matches").DataTable({
        "processing": false, // for show progress bar
        "serverSide": true, // for process server side
        "orderMulti": false, // for disable multiple column at once
        "targets": 'no-sort',
        "bSort": false,
        "bFilter": false,
        "lengthChange": false,
        "order": [],
        "pageLength": 15,

        "ajax": {
            "url": "getmatches",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [
            {
                "targets": -1,
                "data": null,
                "defaultContent": '<button class="pull-right btn btn-sm btn-outline-primary">Goedkeuren</button>'
            }
        ],
        "columns": [
           { "data": "creationDateTime", "name": "Datum", "autoWidth": true },
            { "data": "winner", "name": "Winnaar", "autoWidth": true },
            { "data": "loser", "name": "Verliezer", "autoWidth": true },
            { "data": "rating", "name": "Rating", "autoWidth": true },
            { "data": "status", "name": "Status", "autoWidth": true },
            { "data": null, "autoWidth": true }
        ]
    }); 
    $('#matches tbody').on('click', 'button', function () {
        var data = table.row($(this).parents('tr')).data();
        var post = $.post(`confirm `, { matchId: data.matchId });
        post.done(() => { window.location.href = "overview"; });
    });
});


