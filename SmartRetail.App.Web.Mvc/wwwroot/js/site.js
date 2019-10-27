// global variables
var previousPathInner = "";
var currentPathInner = "";
var image = null;
var previousPath = "";
var currentPath = "";

/// BEGIN SECTION: Product

// modal window for edit the product
$(document).on('click','.compItem',function(e) {

        e.preventDefault();
        $.get(e.currentTarget.href,
            function(data) {
                $('#dialogContent').html(data);
                $('#modDialog').modal('show');
            });
    });

// close modal window for edit the product
function close() {
    $('#Result').text("");
    $('#modDialog').modal('hide');
}

// change image in modal 
$(document).on('click',
    '.imgChange',
    function(s) {
        var input = document.createElement('input');
        input.type = 'file';
        input.onchange = e => {
            var file = e.target.files[0];

            if (isImage(file.type)) {
                var fr = new FileReader();
                fr.onload = function() {
                    $('#addImg').empty();
                    $('#emptyImage').removeAttr('id');
                    $('.innerImage').attr('src', fr.result);

                };
                fr.readAsDataURL(file);
                image = file;
            }
        };

        input.click();
    }
);

// image types
function isImage(type) {
    switch (type) {
        case "image/jpeg":
        case "image/png":
        case "image/jpg":
        case "image/raw":
        case "image/tiff":
        case "image/bmp":
        case "image/gif":
        case "image/jp2":
        case "image/pcx":
        case "image/ico":
            return true;
        default:
            return false;
    }
}

// update product in modal window
function updateItem(id) {
    flag = false;
    prodName = document.getElementsByName('ProdName')[0].value.toString();
    price = document.getElementsByName('Price')[0].value;
    
    if (prodName.trim() == '') {
        document.getElementById('prodNameValidation').innerHTML = 'Необходимо ввести наименование!';
        setTimeout(() => { document.getElementById('prodNameValidation').innerHTML = ''; }, 1000);
        flag = true;
    }
    if (isNaN(price)) {
        document.getElementById('priceValidation').innerHTML = 'Не число!';
        setTimeout(() => { document.getElementById('priceValidation').innerHTML = ''; }, 1000);
        flag = true;
    }
    if (flag) {
        return;
    }

    var itemId = id;
    var item = {
        Id: parseInt(itemId, 10),
        ProdName: prodName,
        Price: parseFloat(price),
        VendorCode: document.getElementsByName('VendorCode')[0].value.toString(),
        Color: document.getElementsByName('Color')[0].value.toString(),
        Size: document.getElementsByName('Size')[0].value.toString(),
        Cost: 0,
        Stock: 0,
        img: image,
        Vendor: "",
        UnitId: parseInt(document.getElementById('unit').value),
        CategoryId: 0
    };

    var formData = getFormData(item);
    formData.append('img', image);

    $.ajax({
        type: "PUT",
        url: "../ProductsPage/Edit",
        data: formData,
        processData: false,
        contentType: false,
        success: function(response) {
            $('#Result').removeClass("text-danger");
            $('#Result').addClass("text-success");
            $('#Result').text('Успешно!');
            setTimeout(close, 1000);

        },
        error: function(thrownError) {
            $('#Result').removeClass("text-success");
            $('#Result').addClass("text-danger");
            $('#Result').text("Не вышло... Попробуйте ещё раз.");
            setTimeout(() => { $('#Result').text(""); }, 1000);

            console.log(thrownError);
        }
    });
}



// for convert json to formdata object
function getFormData(object) {
    var formData = new FormData();
    Object.keys(object).forEach(key => formData.append(key, object[key]));
    return formData;
}


/// END SECTION: Product.


// modal window for categories (folders)
$(document).on('click',
    '.folderModal',
    function(s, e) {
        var item = {
            fullpath: ""
        };

        $.ajax({
            type: 'GET',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            url: "../FoldersTree",
            data: JSON.stringify(item),
            success: function(response) {
                $('#modDialog').modal('hide');
                $('#dialogContentFolder').html(response);
                $('#modFolders').modal('show');
            },
            error: function(thrownError) {
                console.error('Unable to fetch folder list.', thrownError);
            }
        });
    });


// event for next level of folders
$(document).on('click',
    '.folderRow',
    function(s) {
        showLoader();
        s.currentTarget.parentElement.parentElement.style.transition = "all 0.5s";
        s.currentTarget.parentElement.parentElement.bgColor = '#e7ebec';
        previousPathInner = getPreviousPath(s.currentTarget.parentElement.id);
        currentPathInner = s.currentTarget.parentElement.id;
        goInner(currentPathInner);

    });

// event for previous level of folders
$(document).on('click',
    '.escapeButton',
    function(s) {
        showLoader();
        previousPathInner = getPreviousPath(s.currentTarget.id);
        currentPathInner = s.currentTarget.id;
        goInner(currentPathInner);
    });

// go into next level of folders
// @fullpath can make a direction
function goInner(fullPath) {
    var resp;
    var item = {
        fullpath: fullPath
    };
    $.ajax({
        type: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        url: "../FoldersTree",
        data: JSON.stringify(item),
        success: function(response) {
            resp = response;
            reRenderModal(resp);
        },
        error: function(thrownError) {
            console.error('Unable to goIn.', thrownError)
        }
    });

    return false;
}

// render new folders
function reRenderModal(newLevel) {
    $('#tableBodyFolder').empty();
    $('.escapeButton').remove();
    if (newLevel.folders != null &&
        newLevel.folders.length != 0 &&
        newLevel.folders[0].fullpath.split('/').length > 3) {
        $('.folderTable').before('<div class="mb-3"><button class="btn btn-primary escapeButton" id="' +
            getPreviousPath(newLevel.folders[0].fullpath) +
            '">Назад</button></div>');
    } else if (newLevel.folders == null || newLevel.folders.length == 0) {
        $('.folderTable').before('<div class="mb-3"><button class="btn btn-primary escapeButton" id="' +
            getParentFolder(currentPathInner) +
            '">Назад</button></div>');
    }

    for (var i = 0; i < newLevel.folders.length; i++) {
        $('#tableBodyFolder').append('<tr><td><div class="row" id="' +
            newLevel.folders[i].fullpath +
            '"><div class="col-3 folderRow"><img src="/img/folder.svg" width="50px" height="50px"></div>' +
            '<div class="col-6 folderRow">' +
            '<p><b>' +
            newLevel.folders[i].folder +
            '</b></p></div></div></td></tr>');
    }
    removeLoader();

}

// method that makes a previous path from path
function getPreviousPath(fullpath) {
    var path = fullpath.split('/');
    var newPath = "";
    for (var i = 1; i < path.length - 2; i++) {
        newPath += ("/" + path[i]);
    }
    return newPath;
}

// call if there are no children (folders)
function getParentFolder(fullpath) {
    var path = fullpath.split('/');
    var newPath = "";
    for (var i = 1; i < path.length - 1; i++) {
        newPath += ("/" + path[i]);
    }
    return newPath;
}

// modal window fo changing folder names
$(document).on('click',
    '.changeFolderName',
    function(e) {
        e.preventDefault();
        $.get(this.href,
            function(data) {
                $('#dialogContent').html(data);
                $('#modDialog').modal('show');
            });
    });

// call if folder name is changed
function changeFolderNameSubmit(fullpath) {
    var item = {
        pathtofolder: fullpath,
        newfoldername: document.getElementById('newFolderName').value.toString()
    };

    $.ajax({
        type: "PUT",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        url: "../FolderRename",
        data: JSON.stringify(item),
        success: function(response) {
            $('#Result').removeClass("text-danger");
            $('#Result').addClass("text-success");
            $('#Result').text('Успешно!');
            setTimeout(close, 1000);

            document.getElementById(fullpath.split('/')[fullpath.split('/').length-1]).children[0].textContent = item.newfoldername;
            document.getElementById(fullpath.split('/')[fullpath.split('/').length - 1]).id =
                item.newfoldername;

            var href = "FolderRename/?path=" + fullpath;

            $('a[href="../FolderRename/?path=' + fullpath + '"]')[0].href = ".." + changeLastElement(href.split('/'), item.newfoldername ,false)

            document.getElementById(fullpath).id =
                changeLastElement(fullpath.split('/'), item.newfoldername, true);

        },
        error: function(thrownError) {
            $('#Result').removeClass("text-success");
            $('#Result').addClass("text-danger");
            $('#Result').text("Не вышло... Попробуйте ещё раз.");
            setTimeout(() => {$('#Result').text("");}, 1000);

            console.log(thrownError);
        }
    });
}

// call for change paths to folders for next folderName update
function changeLastElement(array, newElement, removeFirst) {
    if (removeFirst) {
        array.shift();
    }
    array.pop();
    array.push(newElement);
    return array.map(item => "/" + item).reduce((sum, cur) => sum + cur, "");
}

// open the next level of folders & products
$(document).on('click',
    '.folder',
    function(s) {
        showLoader();
        s.currentTarget.parentElement.parentElement.style.transition = "all 0.5s";
        s.currentTarget.parentElement.parentElement.bgColor = '#e7ebec';
        previousPath = getPreviousPath(s.currentTarget.parentElement.id);
        currentPath = s.currentTarget.parentElement.id;
        goIn(currentPath);
    });

// open the previous level of folders & products
$(document).on('click',
    '.escape',
    function(s) {
        showLoader();
        previousPath = getPreviousPath(s.currentTarget.id);
        currentPath = s.currentTarget.id;
        goIn(currentPath);
    });

// get the level of folders & products
function goIn(fullPath) {
    var resp;
    var item = {
        fullpath: fullPath
    };
    $.ajax({
        type: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        url: "../Groups",
        data: JSON.stringify(item),
        success: function(response) {
            resp = response;
            reRender(resp);
        },
        error: function(thrownError) {
            console.error('Unable to goIn.', thrownError)
        }
    });

    return false;
}

// render new folders & products
function reRender(newLevel) {
    $('#tableBody').empty();
    $('.escape').remove();
    if (newLevel.folders.length != 0 && newLevel.folders[0].fullpath.split('/').length > 3) {
        $('#modDialog').after('<div class="mb-3"><button class="btn btn-primary escape" id="' +
            getPreviousPath(newLevel.folders[0].fullpath) +
            '">Назад</button></div>');
    } else if (newLevel.folders == null || newLevel.folders.length == 0) {
        $('#modDialog').after('<div class="mb-3"><button class="btn btn-primary escape" id="' +
            getParentFolder(currentPath) +
            '">Назад</button></div>');
    }

    for (var i = 0; i < newLevel.folders.length; i++) {
        $('#tableBody').append(
            '<tr>' +
            '<td>' +
            '<div class="row" id="' + newLevel.folders[i].fullpath +'">' +
            '<div class="col-3 folder">'+
            '<img src="/img/folder.svg" width="50px" height="50px">'+
            '</div>'+
            '<div class="col-6 folder">'+
            '<p id="'+newLevel.folders[i].folder+'"><b>' + newLevel.folders[i].folder + '</b></p>'+
            '</div>'+
            '<div class="col-3">' +
            '<div class="dropdown">' +
            '<button class="btn btn-primary" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">...</button>' +
            '<div class="dropdown-menu" aria-labelledby="dropdownMenuButton">' +
            '<a class="dropdown-item changeFolderName" href="../FolderRename/?path='+newLevel.folders[i].fullpath+'" >Изменить</a>' +
            '<a class="dropdown-item" href="#">Переместить</a><a class="dropdown-item" href="#">Скопировать</a>' +
            '<a class="dropdown-item" href="#">Удалить</a></div></div></div></div>' +
            '</td>' +
            '</tr>');
    }



    if (newLevel.products != null && newLevel.products.length != 0) {
        for (var i = 0; i < newLevel.products.length; i++) {
            var prod = newLevel.products[i];
            var img = prod.imgUrl.split('/').length == 0 ? "/img/default.JPG" : prod.imgUrl;
            $('#tableBody').append(
                '<tr><td>' +
                '<div class="row"><div class="col-3">' +
                '<img src="' + img + '" width="80px" height="80px" alt="' + prod.prodName + '" class="img-circle">' +
                '</div>' +
                '<div class="col-6"><b>'+ prod.prodName +'</b></div>' +
                '<div class="col-3">' + '<a class="compItem btn btn-primary" href="/Edit?id=' + prod.id +'">Изменить</a></div>' +
                '</td></tr>');
        }
    }
    removeLoader();
}

/*<div class="row">
                    <div class="col-3">
                        @{
                            var img = !string.IsNullOrEmpty(@item.ImgUrl) ? @item.ImgUrl : "/img/default.png";
                        }
                        <img src="@img" width="80px" height="80px" alt="@item.ProdName" class="img-circle">
                    </div>
                    <div class="col-6"><b>
                        @Html.DisplayFor(modelItem => item.ProdName)
                    </b></div>
                    <div class="col-3">
                        @Html.ActionLink("Изменить", "Edit",
                            new {id = item.Id}, new {@class = "compItem btn btn-primary"})
                    </div>
                </div>*/

function closeFolderModal() {
    $('#modFolders').modal('hide');
    $('#modDialog').modal('show');
}


function showLoader()
{
    $('#mainDiv').addClass('overlay');
    $('body').append('<div style="" id="loadingDiv"><div class="loader">Loading...</div></div>');
}

function removeLoader1(){
    $( "#loadingDiv" ).remove();
    $('#mainDiv').removeClass('overlay');
}

function removeLoader(){
    $( "#loadingDiv" ).fadeOut(100, function() {
        // fadeOut complete. Remove the loading div
        $( "#loadingDiv" ).remove(); //makes page more lightweight 
        $('#mainDiv').removeClass('overlay');
    });
}