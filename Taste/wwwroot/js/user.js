let dataTable;

$(document).ready(() => {
	loadList();
});

const loadList = () => {
	dataTable = $('#DT_Load').DataTable({
		ajax: {
			url: '/api/users',
			type: 'GET',
			datatype: 'json'
		},
		columns: [
			{ data: 'fullName', width: '25%' },
			{ data: 'email', width: '25%' },
			{ data: 'phoneNumber', width: '25%' },
			{
				data: { id: "id", lockoutEnd: "lockoutEnd" },
				render: function (data) {
					let today = new Date().getTime();
					let lockout = new Date(data.lockoutEnd).getTime();
					if (lockout > today) {
						return `<div class="text-center">
                                     <a class="btn btn-danger" style="cursor:pointer; witdth:100%;" onclick="lockUnlock('${data.id}')">
                                        <i class="fas fa-lock-open"></i> Unlock
                                     </a>
                                </div>`;
					} else {
						return `<div class="text-center">
                                     <a class="btn btn-success" style="cursor:pointer; witdth:100%;" onclick="lockUnlock('${data.id}')">
                                        <i class="fas fa-lock"></i> Lock
                                     </a>
                                </div>`;
					}
				}

			}

		],
		language: {
			emptyTable: 'no data found'
		},
		width: '100%'
	});
}

const lockUnlock = (id) => {

	$.ajax({
		type: 'POST',
		url: '/api/users',
		data: JSON.stringify(id),
		contentType: 'application/json',
		success: function (data) {
			if (data.success) {
				toastr.success(data.message);
				dataTable.ajax.reload();
			} else {
				toastr.error(data.message);
			}
		}
	});

}