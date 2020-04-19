/*
* Cần có 1 số thư viện sau:
* + jquery
* + bootbox
* + select2
*/

// Công cụ khác
function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

// Xử lý chuỗi
function update_slug(src,dest) {
    $(dest).val(string_to_slug($(src).val().trim()));
}
function string_to_slug(str) {
    if (str == null || typeof (str) == 'undefined') return '';
    str = str.toString();
    str = str.toLowerCase();
    str = str.replace(/^\s+|\s+$/g, ''); // trim

    // remove accents, swap ñ for n, etc
    var from = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴàáäâèéëêìíïîòóöôùúüûñç·/_:;";
    var to = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyaaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyaaaaeeeeiiiioooouuuunc-----";
    for (var i = 0, l = from.length; i < l; i++) {
        str = str.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i));
    }

    str = str.replace(/[^a-z0-9 -]/g, '') // remove invalid chars
        .replace(/\s+/g, '-') // collapse whitespace and replace by -
        .replace(/-+/g, '-') // collapse dashes
        .replace(' ', '-')
        .replace('--', '-')
        .replace('"', '');

    return str;
}
function clone_val(src, dest) {
    $(dest).val($(src).val().trim());
}

// Xử lý ajax
function ajax_post(url, data, redirect_type = -1, ctrler = '', reload = false, success_func, error_func) {
    $.ajax({
        data: data,
        type: 'POST',
        datatype: 'json',
        url: url,
        success: function (responsive) {
            if (reload)
                window.location.reload();
            else if (redirect_type == -1) {
                bootbox.alert("Đã cập nhật");
            } else if (redirect_type == 0) {
                window.location.replace('/' + ctrler);
            } else if (redirect_type == 1) {
                window.location.replace('/' + ctrler + '/Create');
            } else if (redirect_type == 2) {
                window.location.replace('/' + ctrler + '/Edit/' + responsive.newid);
            }
            if (typeof success_func != 'undefined') {
                success_func();
            }
        },
        error: function (responsive) {
            bootbox.alert("Có lỗi xảy ra!");
            if (typeof error_func != 'undefined') {
                error_func();
            }
        }
    });
}
function confirm_ajax_post(title, msg, url, data, redirect_type = -1, ctrler = '', success_func, error_func) {
    bootbox.confirm({
        title: title,
        message: msg,
        buttons: {
            confirm: {
                className: 'btn-danger'
            },
            cancel: {
                className: 'btn-success'
            }
        },
        callback: function (result) {
            if (result == true) {
                ajax_post(url, data, redirect_type = -1, ctrler = '', true, success_func, error_func);
            }
        }
    });
}
function confirm_ajax_post(title, msg, confirm_label, cancel_label, url, data, redirect_type = -1, ctrler = '', reload = false, success_func, error_func) {
    bootbox.confirm({
        title: title,
        message: msg,
        buttons: {
            confirm: {
                label: confirm_label,
                className: 'btn-danger'
            },
            cancel: {
                label: cancel_label,
                className: 'btn-success'
            }
        },
        callback: function (result) {
            if (result == true) {
                //console.log('vao dong 108');
                ajax_post(url, data, redirect_type, ctrler, reload, success_func, error_func);
            }
        }
    });
}

// Xử lý lọc, cha-con
function filterChilds(itm, childname) {
    var pid = $(itm).val();
    $("#" + childname + " > option").each(function () {
        if (pid == $(this).data("parent")) {
            $(this).show();
            $(this).prop("disabled", false);
        } else {
            $(this).hide();
            $(this).prop("disabled", true);
        }
    });
    $("#" + childname + " > option").first().prop("disabled", false);
    $("#" + childname).val(0).trigger('change');
    $("#" + childname).select2("destroy");
    $("#" + childname).select2({});
}
