var ToolTip = {
    transitionInterval: null,
    transitionDelayTimeout: null,
    delay: null,
    fadeDuration: null,
    initialized: false,

    init: function(options){
        var settings = $.extend({
            delay: 400,
            fadeDuration: 250,
            fontSize: '1.0em',
            theme: 'dark',                   //  or light
            textColor: '#fff',
            shadowColor: '#000',
            fontFamily: 'Arial, Helvetica, sans-serif'
        }, options);

        settings.theme = settings.theme.toLowerCase();
        if (settings.theme == 'dark' && typeof(options.textColor) === 'undefined')
        {
            settings.textColor = '#fff';
        }
        else if (settings.theme == 'light' && typeof(options.textColor) === 'undefined')
        {
            settings.textColor = '#333';
        }

        ToolTip.delay = settings.delay;
        ToolTip.fadeDuration = settings.fadeDuration;

        $(document).on('mouseenter', '.tip-hotspot', function(e){
            $(this).data('tip-settings', settings);
            clearInterval(ToolTip.transitionInterval);
            clearTimeout(ToolTip.transitionDelayTimeout);
            var tipText = $(this).attr('data-tip');
            if (typeof tipText !== 'undefined')
            {
                var newTipId = 'tip-bubble-' + new Date().getTime();
                var tip = $('<div id="' + newTipId + '" class="tip-bubble">' + tipText + '</div>');
                tip.appendTo('body');

                var tipElement = $('#' + newTipId);
                $(this).data('tip-bubble-id', newTipId);

                var tipWidth = tipElement.width() + parseInt(tipElement.css('padding-left')) + parseInt(tipElement.css('padding-right'));
                var tipHeight = tipElement.height() + parseInt(tipElement.css('padding-top')) + parseInt(tipElement.css('padding-bottom'));;
                var leftPos = $(this).offset().left + $(this).width() / 2 - tipWidth / 2;
                var topPos = $(this).offset().top + $(this).height() + parseInt($(this).css('padding-top')) + parseInt($(this).css('padding-bottom')) + 10;
                var position = 'CENTER';
                var arrowPlacement = 'TOP';

                if (leftPos + tipWidth > $(window).width())
                {
                    leftPos = $(window).width() - tipWidth - 10;
                    position = 'RIGHT';
                }
                else if (leftPos <= 0)
                {
                    leftPos = 20;
                    position = 'LEFT';
                }

                if (topPos + tipHeight >= $(window).height())
                {
                    topPos = $(this).offset().top - tipHeight - 10;
                    arrowPlacement = 'BOTTOM';
                }
                else if (topPos <= 0)
                {
                    arrowPlacement = 'TOP';
                }

                tip.css({
                    top: topPos,
                    left: leftPos,
                    'font-size': settings.fontSize,
                    'background-color': (settings.theme == 'macarons') ? '#000' : '#fff',
                    color: settings.textColor,
                    'font-family': settings.fontFamily,
                    'box-shadow': '0px 3px 10px ' + settings.shadowColor
                });


                if (settings.theme == 'macarons')
                {
                    tipElement.addClass('tip-theme-dark');
                }
                else
                {
                    tipElement.addClass('tip-theme-light');
                }


                if (position == 'RIGHT')
                {
                    //console.log('right');
                    tipElement.removeClass('tip-bubble-center-point').removeClass('tip-bubble-left-point').addClass('tip-bubble-right-point');
                }
                else if (position == 'LEFT')
                {
                    //console.log('left');
                    tipElement.removeClass('tip-bubble-center-point').removeClass('tip-bubble-right-point').addClass('tip-bubble-left-point');
                }
                else
                {
                    //console.log('center');
                    tipElement.removeClass('tip-bubble-right-point').removeClass('tip-bubble-left-point').addClass('tip-bubble-center-point');
                }

                if (arrowPlacement == 'TOP')
                {
                    //console.log('top');
                    tipElement.removeClass('tip-arrow-bottom').addClass('tip-arrow-top');
                }
                else
                {
                    //console.log('bottom');
                    tipElement.removeClass('tip-arrow-top').addClass('tip-arrow-bottom');
                }

                ToolTip.transitionDelayTimeout = setTimeout(function(){
                    ToolTip.transitionInterval = setInterval(function(){
                        var newOpacity = parseFloat(tipElement.css('opacity')) + (1 / ToolTip.fadeDuration * 10);
                        //console.log(newOpacity);
                        tipElement.css('opacity', newOpacity);
                        if (newOpacity == 1)
                        {
                            clearInterval(ToolTip.transitionInterval);
                        }
                    }, 1);
                }, ToolTip.delay);
            }
        });

        $(document).on('mouseleave', '.tip-hotspot', function(e){
            clearInterval(ToolTip.transitionInterval);
            clearTimeout(ToolTip.transitionDelayTimeout);
            var tipElementId = $(this).data('tip-bubble-id');
            $('#' + tipElementId).remove();
        });

        ToolTip.initialized = true;
    }
};


$(function($){
    var methods = {
        init: function(options){
            var settings = $.extend({
                text: null,
                delay: 0,
                fadeDuration: 250,
                fontSize: '1.0em',
                theme: 'dark',                                  //  or light
                textColor: '#fff',
                shadowColor: '#000',
                fontFamily: 'Arial, Helvetica, sans-serif'
            }, options);

            settings.transitionInterval = null;
            settings.transitionDelayTimeout = null;

            if (settings.theme == 'dark' && typeof(options.textColor) === 'undefined')
            {
                settings.textColor = '#fff';
            }
            else if (settings.theme == 'light' && typeof(options.textColor) === 'undefined')
            {
                settings.textColor = '#333';
            }

            this.data('tip-settings', settings);

            var element = this;
            if (element.hasClass('tip-hotspot') && ToolTip.initialized)
            {
                console.log('HTML hover tip parameters found for the target element (' + element.attr('id') + '). Ignoring javascript settings.');
            }

            if (settings.text == null || $.trim(settings.text) == '')
            {
                //$.error('jQuery.tooltip error -> text cannot be null or empty.');
            }

            element.on('mouseenter', function(e){
                showTip(element);
            });

            element.on('mouseleave', function(e){
                removeTip(element);
            });
        },

        setText: function(text){
            this.data('tip-settings').text = text;
        },

        autoTip: function(options){
            var settings = $.extend({
                displayDuration: 5000,
                fadeOutDuration: 1000,
                onShowCallback: null,
                onHideCallback: null
            }, options);
            var element = this;

            showTip(element, settings.onShowCallback);
            setTimeout(function(){
                removeTip(element, {
                    fadeOutDuration: settings.fadeOutDuration,
                    callback: settings.onHideCallback
                });
            }, settings.displayDuration);
        }
    };

    $.fn.tooltip = function(methodOrOptions){
        if (methods[methodOrOptions])
        {
            return methods[methodOrOptions].apply(this, Array.prototype.slice.call(arguments, 1));
        }
        else if (typeof methodOrOptions === 'object' || !methodOrOptions)
        {
            return methods.init.apply(this, arguments);
        }
        else
        {
            $.error('Method ' + methodOrOptions + ' does not exist for jQuery.tooltip');
        }
    };

    function showTip(htmlElement, onShowCallback){
        var settings = htmlElement.data('tip-settings');

        clearInterval(settings.transitionInterval);
        clearTimeout(settings.transitionDelayTimeout);

        var newTipId = 'tip-bubble-' + new Date().getTime();
        var tip = $('<div id="' + newTipId + '" class="tip-bubble">' + settings.text + '</div>');
        tip.appendTo('body');

        var tipElement = $('#' + newTipId);
        htmlElement.data('tip-bubble-id', newTipId);

        var tipWidth = tipElement.width() + parseInt(tipElement.css('padding-left')) + parseInt(tipElement.css('padding-right'));
        var tipHeight = tipElement.height() + parseInt(tipElement.css('padding-top')) + parseInt(tipElement.css('padding-bottom'));;
        var leftPos = htmlElement.offset().left + htmlElement.width() / 2 - tipWidth / 2;
        var topPos = htmlElement.offset().top + htmlElement.height() + parseInt(htmlElement.css('padding-top')) + parseInt(htmlElement.css('padding-bottom')) + 10;
        var position = 'CENTER';
        var arrowPlacement = 'TOP';

        if (leftPos + tipWidth > $(window).width())
        {
            leftPos = $(window).width() - tipWidth - 10;
            position = 'RIGHT';
        }
        else if (leftPos <= 0)
        {
            leftPos = 20;
            position = 'LEFT';
        }

        if (topPos + tipHeight >= $(window).height())
        {
            topPos = htmlElement.offset().top - tipHeight - 10;
            arrowPlacement = 'BOTTOM';
        }
        else if (topPos <= 0)
        {
            arrowPlacement = 'TOP';
        }

        tipElement.css({
            top: topPos,
            left: leftPos,
            'font-size': settings.fontSize,
            'background-color': (settings.theme == 'macarons') ? '#000' : '#fff',
            color: settings.textColor,
            'font-family': settings.fontFamily,
            'box-shadow': '0px 3px 10px ' + settings.shadowColor
        });


        if (settings.theme == 'macarons')
        {
            tipElement.addClass('tip-theme-dark');
        }
        else
        {
            tipElement.addClass('tip-theme-light');
        }


        if (position == 'RIGHT')
        {
            //console.log('right');
            tipElement.removeClass('tip-bubble-center-point').removeClass('tip-bubble-left-point').addClass('tip-bubble-right-point');
        }
        else if (position == 'LEFT')
        {
            //console.log('left');
            tipElement.removeClass('tip-bubble-center-point').removeClass('tip-bubble-right-point').addClass('tip-bubble-left-point');
        }
        else
        {
            //console.log('center');
            tipElement.removeClass('tip-bubble-right-point').removeClass('tip-bubble-left-point').addClass('tip-bubble-center-point');
        }

        if (arrowPlacement == 'TOP')
        {
            //console.log('top');
            tipElement.removeClass('tip-arrow-bottom').addClass('tip-arrow-top');
        }
        else
        {
            //console.log('bottom');
            tipElement.removeClass('tip-arrow-top').addClass('tip-arrow-bottom');
        }

        settings.transitionDelayTimeout = setTimeout(function(){
            settings.transitionInterval = setInterval(function(){
                var newOpacity = parseFloat(tipElement.css('opacity')) + (1 / settings.fadeDuration * 10);
                //console.log(newOpacity);
                tipElement.css('opacity', newOpacity);
                if (newOpacity == 1)
                {
                    clearInterval(settings.transitionInterval);
                    if (typeof onShowCallback === 'function')
                    {
                        onShowCallback.call();
                    }
                }
            }, 1);
        }, settings.delay);
    }

    function removeTip(htmlElement, options){
        var removeSettings = $.extend({
            fadeOutDuration: 0,
            callback: null
        }, options);

        var settings = htmlElement.data('tip-settings');
        clearInterval(settings.transitionInterval);
        clearTimeout(settings.transitionDelayTimeout);
        if (removeSettings.fadeOutDuration == null || removeSettings.fadeOutDuration == 0)
        {
            var tipElementId = htmlElement.data('tip-bubble-id');
            $('#' + tipElementId).remove();
        }
        else
        {
            var tipElementId = htmlElement.data('tip-bubble-id');
            var tipElement = $('#' + tipElementId);

            settings.transitionInterval = setInterval(function(){
                var newOpacity = parseFloat(tipElement.css('opacity')) - (1 / removeSettings.fadeOutDuration * 10);
                //console.log(newOpacity);
                tipElement.css('opacity', newOpacity);
                if (newOpacity == 0)
                {
                    clearInterval(settings.transitionInterval);
                    tipElement.remove();
                    htmlElement.removeData('tip-bubble-id');
                    if (removeSettings.callback != null && typeof removeSettings.callback === 'function')
                    {
                        removeSettings.callback.call();
                    }
                }
            }, 1);

        }
    }
}(jQuery));