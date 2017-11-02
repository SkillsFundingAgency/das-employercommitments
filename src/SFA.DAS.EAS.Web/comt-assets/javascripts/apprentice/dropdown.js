(function () {

    // https://select2.github.io/examples.html
    var init = function () {
        if ($("select#TrainingCode")) {
            $("select#TrainingCode").select2();
        }
    };
    init();

    // open dropdownon on focus
    $(document).on('focus', '.select2', function () {
        $(this).siblings('select').select2('open');
    });

    // retain tabbed order after selection
    $('select#TrainingCode').on('select2:select', function () {
        $("#StartDate_Month").focus();
    });

    // retain tabbed order on close without selection
    $('select#TrainingCode').on('select2:close', function () {
        $("#StartDate_Month").focus();
    });

}());