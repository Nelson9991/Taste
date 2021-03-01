let dataTable;

$(document).ready(() => {
    let url = window.location.search;
    if (url.includes('cancelled')) {
        loadList('cancelled');
    }
    else if (url.includes('completed')) {
        loadList('completed');
    }
    else if (url.includes('inProcess')) {
        loadList('inProcess');
    } else {
        loadList('');
    }
});

const loadList = (param) => {
    dataTable = $('#DT_Load').DataTable({
        ajax: {
            url: '/api/order?status=' + param,
            type: 'GET',
            datatype: 'json'
        },
        columns: [
            { data: 'orderHeader.id', width: '15%' },
            { data: 'orderHeader.pickUpName', width: '25%' },
            { data: 'orderHeader.applicationUser.email', width: '20%' },
            { data: 'orderHeader.orderTotal', width: '20%' },
            {
                data: 'orderHeader.id',
                render: function (data) {
                    return `
<div class="d-flex flex-row justify-content-center">
                             <div class="text-center" style="margin-right:10px;">
                                <a href="/Admin/Order/OrderDetails?id=${data}" class="btn btn-success" style="width: 100px;">
                                 <i class="far fa-edit"></i>
                                 Details
                               </a>
                             </div>
</div>`;
                },
                width: '20%'
            }
        ],
        language: {
            emptyTable: 'no data found'
        },
        width: '100%'
    });
}
