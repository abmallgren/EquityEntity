// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function HideModal(selector) {
    $(selector).modal('hide');
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();
    $('body').css('paddingRight', 0);
}
function ShowModal(selector) {
    $(selector).modal('show');
}
function ShowInformationModal(content) {
    $('#informationModal .modal-body').html(content);
    $('#informationModal').modal('show');
}
function ApplyDataTable(selector) {
    $(selector).DataTable({searching:false});
}
function UnselectOptions(selector) {
    $.each($(selector + ' option:selected'), function () {
        $(this).prop('selected', false);
    });
}
function InitTinyMce(selector) {
    tinymce.init({
        selector: selector,
        plugins: 'anchor autolink autosave charmap codesample emoticons image link lists media searchreplace table visualblocks wordcount checklist mediaembed casechange export formatpainter pageembed linkchecker a11ychecker tinymcespellchecker permanentpen powerpaste advtable advcode editimage tinycomments tableofcontents footnotes mergetags autocorrect typography inlinecss',
        toolbar: 'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | link image media table mergetags | addcomment showcomments | spellcheckdialog a11ycheck typography | align lineheight | checklist numlist bullist indent outdent | emoticons charmap | removeformat',
        tinycomments_mode: 'embedded',
        tinycomments_author: 'Author name',
        mergetags_list: [
            { value: 'First.Name', title: 'First Name' },
            { value: 'Email', title: 'Email' },
        ]
    });
}