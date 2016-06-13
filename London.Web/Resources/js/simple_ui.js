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

//****************************
//      CARD
//****************************

var toggleCard = function ($) {
    var active = 'active';

    function expendCard($this) {
        $this.addClass(active);
        $('#' + $this.data().content).addClass(active);
    }

    function hideCard($this) {
        $this.removeClass(active);
        $('#' + $this.data().content).removeClass(active);
    }

    function isActive($this) {
        return $this.hasClass(active);
    }

    return function (el) {
        var $this = $(el);

        if (isActive($this)) {
            hideCard($this);
        } else {
            expendCard($this);
        }
    };
}(jQuery);