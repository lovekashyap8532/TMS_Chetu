//< !--Modal User Update Script-- >
        $(document).ready(function() {
            $("#btnSubmit").click(function () {
                var data = {
                    Id: $("#hdnTskID").val(),
                    Status: $("#Status").val(),
                    Description: $("#Description").val()
                };

                $.ajax({
                    type: "POST",
                    url: "/Home/EditUserTask",
                    data: data,
                    dataType: "json", // Expecting JSON response
                    success: function (response) {
                        if (response.success) {
                            alert("Data Saved Successfully !!");
                            // Close the modal
                            $("#modal-default").modal("hide");
                            // Clear the modal data
                            clearModalData();
                        } else {
                            alert("Error: " + response.message);
                        }
                    },
                    error: function (xhr, status, error) {
                        alert("Please fill the description !!: " + error);
                    }
                });
            });
    });
        function ShowPopup(element) {
        var id = $(element).data('id');
        var title = $(element).data('title');
        var status = $(element).data('status');

        $("#hdnTskID").val(id);
        $("#modalTitle").text(title);
        $("#Status").val(status);

        $("#modal-default").modal("show");
    }


        function clearModalData() {
            $("#hdnTskID").val("0");
        $("#modalTitle").text("Title");
        $("#Status").val("InProgress");
        $("#Description").val("");
    }

        function loadTaskProgress(element) {
        var taskId = $(element).data('id');

        $.ajax({
            url: '/Home/ShowTaskProgress',
        type: 'GET',
        data: {id: taskId },
        success: function (data) {
            console.log("Received data:", data); // Debugging log

        // Ensure data is an array
        if (!Array.isArray(data) || data.length === 0) {
            $('#taskProgressTableBody').html('<tr><td colspan="4">No task progress found</td></tr>');
        $('#taskProgressModal').modal('show');
        return;
                }

        $('#taskProgressTableBody').empty(); // Clear previous data

        // Append rows dynamically
        var a=0;
        data.forEach(function (task) {
            a++;
        var row = `<tr>
            <td>${a || 'N/A'}</td>
            <td>${task.updatedDate ? new Date(task.updatedDate).toLocaleString() : 'N/A'}</td>
            <td>${task.taskStatus || 'N/A'}</td>
            <td>${task.description || 'N/A'}</td>
        </tr>`;
        $('#taskProgressTableBody').append(row);
                });

        // Show the modal
        $('#taskProgressModal').modal('show');
            },
        error: function (xhr) {
            console.log("Error Response:", xhr.responseText); // Debugging error
        alert('An error occurred while loading the task progress details.');
            }
        });
    }
