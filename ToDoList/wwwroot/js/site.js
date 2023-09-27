function deleteTodo(item){
    $.ajax({
        url: 'Home/Delete',
        type: 'POST',
        data: { id: item },
        success: function(){
            window.location.reload();
        }
    })
}

function populateForm(item){
    $.ajax({
        url: 'Home/PopulateForm',
        type: 'GET',
        data: { Id: item },
        dataType: 'json',
        success: function(response){
            $('#Todo_Name').val(response.name);
            $('#Todo_Id').val(response.id);
            $('#form-button').val("Update Todo")
            $('#form-action').attr("action", "/Home/Update")
        }
    })
}