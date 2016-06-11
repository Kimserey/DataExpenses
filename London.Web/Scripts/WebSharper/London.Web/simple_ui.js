//****************************
//      CARD
//****************************

(function ($) {
    var _active_ = 'active';

    function expendCard ($this) {
        $this.addClass(_active_);
        $('#' + $this.data().content).addClass(_active_);
    }

    function hideCard ($this) {
        $this.removeClass(_active_);
        $('#' + $this.data().content).removeClass(_active_);
    }

    $(document).ready(function () {
        $('.card-list-item').click(function () {
            if ($(this).hasClass(_active_)) {
                hideCard($(this));
            } else {
                expendCard($(this));
            }
        });
    });
}(jQuery));


//****************************
//      SIDE MENU
//****************************

(function ($) {
    $(document).ready(function () {
        var _show_ = 'show';
        var $sidemenu = $('#side-menu');
        var $mask = $('#mask');

        $('#side-menu-button').click(function () {
            $sidemenu.addClass(_show_);
            $mask.addClass(_show_);
        });

        $mask.click(function () {
            $sidemenu.removeClass(_show_);
            $mask.removeClass(_show_);
        })
    });
}(jQuery));