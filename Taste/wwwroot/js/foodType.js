let dataTable;

$(document).ready(() => {
	loadList();
});

const loadList = () => {
	dataTable = $('#DT_Load').DataTable({
		ajax: {
			url: '/api/FoodType',
			type: 'GET',
			datatype: 'json'
		},
		columns: [
			{ data: 'name', width: '50%' },
			{
				data: 'id',
				render: function (data) {
					return `
<div class="d-flex flex-row justify-content-center">
                             <div class="text-center" style="margin-right:10px;">
                                <a href="/Admin/FoodType/Upsert?id=${data}" class="btn btn-success"               style="width: 100px;">
                                 <i class="far fa-edit"></i>
                                 Edit
                               </a>
                             </div>
                             <div class="text-center">
                                <a class="btn btn-danger" style="width: 100px;" onclick=Delete('/api/foodType?id='+${data})>
                                 <i class="far fa-trash-alt"></i>
                                 Delete
                               </a>
                             </div>
</div>`;
				},
				width: '40%'
			}
		],
		language: {
			emptyTable : 'no data found'
		},
		width : '100%'
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