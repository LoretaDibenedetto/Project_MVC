
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
dataTable = $('#tblData').DataTable({
    ajax: '/api/myData'
});
}


