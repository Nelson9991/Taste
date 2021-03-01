let dataTable;

$(document).ready(() => {
	loadList();
});

const loadList = () => {
	dataTable = $('#DT_Load').DataTable({
		ajax: {
			url: '/api/menuItem',
			type: 'GET',
			datatype: 'json'
		},
		columns: [
			{ data: 'name', width: '25%' },
			{ data: 'price', width: '15%' },
			{ data: 'category.name', width: '15%' },
			{ data: 'foodType.name', width: '15%' },
			{
				data: 'id',
				render: function (data) {
					return `
<div class="d-flex flex-row justify-content-center">
                             <div class="text-center" style="margin-right:10px;">
                                <a href="/Admin/MenuItem/Upsert?id=${data}" class="btn btn-success"               style="width: 100px;">
                                 <i class="far fa-edit"></i>
                                 Edit
                               </a>
                             </div>
                             <div class="text-center">
                                <a class="btn btn-danger" style="width: 100px;" onclick=Delete('/api/menuItem/${data}')>
                                 <i class="far fa-trash-alt"></i>
                                 Delete
                               </a>
                             </div>
</div>`;
				},
				width: '30%'
			}
		],
		language: {
			emptyTable : 'no data found'
		},
		width : '100%',
		order: [[2, "asc"]]
	});
}

const Delete = (url) => {
	swal.fire({
		title: 'Are you sure you want to Delete?',
		text: 'You will not be able to restore the data!',
		icon: 'warning',
		showCancelButton: true
	})
		.then((result) => {
			if (result.isConfirmed) {
				$.ajax({
					type: 'DELETE',
					url: url,
					success: function(data) {
						if (data.success) {
							toastr.success(data.message);
							dataTable.ajax.reload();
						} else {
							toastr.error(data.message);
						}
					}
			});
			}
		});
}