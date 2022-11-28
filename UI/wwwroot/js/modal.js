function openModal(parameters) {
    const id = parameters.data;
    const url = parameters.url;
    const modal = $('#modal');

    if (id === undefined || url === undefined) {
        alert('Error')
        return;
    }

    $.ajax({
        type: 'GET',
        url: url,
        data: { "id": id},
        success: function (response) {
            modal.find(".modal-body").html(response);
            modal.modal('show')
        },
        failure: function () {
            modal.modal('hide')
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
};

//function openModalReport(parameters) {
//    const userId = parameters.userId
//    const reportId = parameters.reportId;
//    const url = parameters.url;
//    const modal = $('#modal');

//    if (userId === undefined || url === undefined || reportId === undefined) {
//        alert('Error')
//        return;
//    }

//    $.ajax({
//        type: 'GET',
//        url: url,
//        data: { "userId": userId, "reportId": reportId },
//        success: function (response) {
//            modal.find(".modal-body").html(response);
//            modal.modal('show')
//        },
//        failure: function () {
//            modal.modal('hide')
//        },
//        error: function (response) {
//            alert(response.responseText);
//        }
//    });
//};
