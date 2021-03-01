let dataTable;

$(document).ready(() => {
	loadList();
});

const loadList = () => {
	dataTable = $('#DT_Load').dataTable({
		ajax: {
			url: '/api/category',
			type: 'GET',
			datatype: 'json'
		},
		columns: [
			{ data: 'name', width: '40%' },
			{ data: 'displayOrder', width: '30%' },
			{
				data: 'id',
				render: function (data) {
					return `
                             <div class="text-center">
                                <a href="/Admin/Category/Upsert?id=${data}" class="btn btn-success"               style="with: 100px;">
                                 <i class="far fa-edit">
                                 Edit
                               <a/>
                             </div>
                             <div class="text-center">
                                <button" class="btn btn-danger" style="with: 100px;">
                                 <i class="far fa-trash-alt">
                                 Delete
                               <a/>
                             </div>`;
				},
				width: '30%'
			}
		],
		language: {
			emptyTable : 'no data found'
		},
		width : '100%'
	});
}