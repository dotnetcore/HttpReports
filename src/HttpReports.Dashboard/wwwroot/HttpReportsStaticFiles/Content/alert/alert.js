/**

	姓名：冯宇
	site: http://fy.035k.com
	更新时间 ：2018-04-02
	更新内容 ：添加actionsheet
	QQ : 515184405
	github : https://github.com/515184405/alert 还有一些其他的项目哦，欢迎star

**/

;(function(){
	var zIndex = 999999;
	var jqueryAlert = function(opts){
		// 设置默认参数
		var opt = {
			'style'        : 'wap', //移动端和PC端
			'title'        : '',    //标题
			'icon'         : '',
			'content'      : '',	//内容
			'contentTextAlign' : 'center', //内容对齐方式
			'width'        : 'auto', //宽度
			'height'       : 'auto', //高度
			'minWidth'     : '0', //最小宽度
			"className"    : '', //添加类名
			'position'     : 'fixed', //定位方式
			'animateType'  : 'scale',
			'modal'        : false, //是否存在蒙层
			'isModalClose' : false, //点击蒙层是否关闭
			'bodyScroll'   : false, //是否关闭body的滚动条
			'closeTime'    : 3000, //当没有按钮时关闭时间
			'actionsheetCloseText' : '', //当当前样式为actionsheet时，关闭的文字按钮
			"buttons"      : {}, //按钮对象</pre>
		};

		// 参数合并
		var option = $.extend({},opt,opts);

		var dialog = {

		};

		dialog.time = 450;//动画关闭时间
		dialog.init = function(){
			dialog.framework();
		};

		// 事件处理
		function IsPC() {
		    var userAgentInfo = navigator.userAgent;
		    var Agents = ["Android", "iPhone",
		                "SymbianOS", "Windows Phone",
		                "iPad", "iPod"];
		    var flag = true;
		    for (var v = 0; v < Agents.length; v++) {
		        if (userAgentInfo.indexOf(Agents[v]) > 0) {
		            flag = false;
		            break;
		        }
		    }
		    return flag;
		}
		var isHaveTouch = IsPC();
		if(isHaveTouch){
			dialog.event = 'click';
		}else{
			dialog.event = 'touchstart';
		};

		var $modal = $("<div class='alert-modal none'>");
		var $container = $("<div class='alert-container animated'>");
		var $title = $("<div class='alert-title'>"+option.title+"</div>");
		var $content = $("<div class='alert-content'>");
		var $buttonBox = $("<div class='alert-btn-box'>");
		var $closeBtn = $("<div class='alert-btn-close'>×</div>");
		var $actionsheetCloseBtn = $("<p class='alert-btn-p alert-btn-sheet'>"+ option.actionsheetCloseText +"</p>"); 

		if(!!option.content[0] && (1 == option.content[0].nodeType)){
			var $newContent = option.content.clone();
			$content.append($newContent)
		}else{
			$content.html(option.content);
		};

		dialog.framework = function(){

			dialog.buttons = [];
			for(var key in option.buttons){
				dialog.buttons.push(key);
			};
			dialog.buttonsLength = dialog.buttons.length;

			option.style == 'actionsheet' ? $container.append($title) : $container.append($title) .append($content);

			if(option.style == 'pc'){
				$container.append($closeBtn).addClass('pcAlert');
			};

			if(option.style == 'actionsheet'){
				$container.addClass('alert-actionsheet');
			};


			if(option.modal || option.modal == 'true'){
				$('body').append($modal);
				$modal.fadeIn(dialog.time);
				option.bodyScroll && $('body').css('overflow','hidden');
			};
			$('body').append($container);

			// 设置内容的对齐方式
			$content.css({
				'text-align' : option.contentTextAlign
			});

			if(parseInt(option.minWidth) > parseInt($container.css('width'))){
				option.width = option.minWidth;
			};

			$modal.css('position',option.position);
			$modal.css('z-index',zIndex);

			++zIndex;

			if(option.position == 'fixed'){
				$container.css({
					'position' : option.position,
					'left'     : '50%',
					'top'      : '30%',
					'z-index'  : zIndex,
				});
			};
			if(option.position == 'absolute'){
				$container.css({
					'position' : option.position,
					'left'     : $(window).width()/2,
					'top'      : $(window).height()/2 + $(window).scrollTop(),
					'z-index'  : zIndex,
				});
			};

			$container.css('width',option.width);
			$container.css('height',option.height);

			if(option.width == 'auto'){
				$container.css('width',$container[0].clientWidth + 10);
			};

			if(parseInt($(window).height()) <=  parseInt($container.css('height'))){
				$container.css('height',$(window).height());
			};

			// 设置class
			(!!option.className) && $container.addClass(option.className);

		    // 设置button内容
		    var indexs = 0;
			for(var key in option.buttons){
				indexs++;
				var $button = $("<p class='alert-btn-p alert-btn-p"+indexs+"'>"+ key +"</p>");
				if(option.style == 'wap'){
					$button.css({
						'width' : Math.floor(($container[0].clientWidth) / dialog.buttonsLength),
					})
				};
				//绑定点击后的事件
				$button.bind(dialog.event,option.buttons[key]);
				$buttonBox.append($button);

			};

			if(dialog.buttonsLength > 0){
				$container.append($buttonBox);
				$content.css('padding-bottom','46px');
				//处理IE下滚动条的问题
				if (window.navigator.userAgent.indexOf("MSIE")>=1) {
					if($content[0].scrollHeight > $content[0].clientHeight){
						$content.css('height',parseInt($content.css('height')) - 46 );
					}
				};
			};

			if(option.title != ''){
				$content.css('padding-top','42px');
			};

			if(dialog.buttonsLength <= 0 && option.title == ''){
				$container.addClass('alert-container-black');
				if(!!option.icon){
					var img = new Image();
					img.onload = function(){
						$content.before(img);
					}
					img.src = option.icon
					$(img).css({
						'width':'45px',
						'height' : 'auto',
						'display':'block',
						'margin' : '10px auto 0 auto'
					})
					$content.css({
						'padding-top' : '5px'
					})
				};
			};

			 // 设置定位
			$container.css({
				'margin-left' : -parseInt($container.css('width'))/2,
				'margin-top' : -parseInt($container.css('height'))/2,
			});

			if( option.style == 'actionsheet' ){
				$container.css({
					'position' : 'fixed',
					'left'     : 0,
					'top'      : 'auto',
					'bottom'   : '-100%',
					'z-index'  : zIndex,
					'margin'   : 0,
					'width'    : '100%',
					'height'   : 'auto',
				});
				$container.animate({
					'bottom'   : '0',
				},dialog.time);
				
			};

			if(option.style != 'actionsheet' && option.animateType == 'scale'){
				$container.addClass('bounceIn');
			};

			if(option.style != 'actionsheet' && option.animateType == 'linear'){
				$container.addClass('linearTop');
			};

			if(option.actionsheetCloseText){
				$buttonBox.append($actionsheetCloseBtn);
				$(document).delegate('.alert-btn-sheet,.alert-modal','click',function(){
					dialog.close();
				})

			}

			isSelfClose();

		};

		// 判断是否满足自动关闭的条件
		function isSelfClose(){
			if(dialog.buttonsLength <= 0 && option.style == 'wap'){
				setTimeout(function(){
					$container.fadeOut(dialog.time);
					$modal.fadeOut(dialog.time);
					option.bodyScroll && $('body').css('overflow','auto');
				},option.closeTime)
			};
		};

		dialog.toggleAnimate = function(){
			if(option.style != 'actionsheet'){
				if(option.animateType == 'scale'){
					return $container.removeClass('bounceIn').addClass('bounceOut');
				}else if(option.animateType == 'linear'){
					return $container.removeClass('linearTop').addClass('linearBottom');
				}else{
					return $container;
				};
			}else{
				return $container.animate({
					'bottom'   : '-100%',
				},dialog.time);
			}
		};

		dialog.close = function(){
			dialog.toggleAnimate().fadeOut(dialog.time);
			$modal.fadeOut(dialog.time);
			option.bodyScroll && $('body').css('overflow','auto');
		};

		option.style == 'pc' && $closeBtn.bind(dialog.event,dialog.close);
		option.isModalClose && $modal.bind(dialog.event,dialog.close);

		dialog.destroy = function(){
			dialog.toggleAnimate().fadeOut(dialog.time);
			setTimeout(function(){
				$container.remove();
				$modal.remove();
				option.bodyScroll && $('body').css('overflow','auto');
			},dialog.time)
		};
		dialog.show = function(){

			$modal.css('z-index',zIndex);

			++zIndex;

			$container.css({
				'z-index'  : zIndex,
			});
			if( option.style != 'actionsheet' ){
				if(option.animateType == 'scale'){
					$container.fadeIn(dialog.time).removeClass('bounceOut').addClass('bounceIn');
				}else if(option.animateType == 'linear'){
					$container.fadeIn(dialog.time).removeClass('linearBottom').addClass('linearTop');
				}else{
					$container.fadeIn(dialog.time);
				};

				if(option.position == 'absolute'){
					$container.css({
						'top'      : $(window).height()/2 + $(window).scrollTop(),
					})
				};
			}else{
				$container.fadeIn(0).animate({
					'bottom'   : '0',
				},dialog.time);
					
			}

			$modal.fadeIn(dialog.time);
			option.bodyScroll && option.modal && $('body').css('overflow','hidden');
			isSelfClose();
		};

		dialog.init();
		 
		return dialog

	}
    if (typeof module !== 'undefined' && typeof exports === 'object') {
        module.exports = jqueryAlert; 
    } else if (typeof define === 'function' && (define.amd || define.cmd)) {
        define(function() { return jqueryAlert; });
    } else {
        this.jqueryAlert = jqueryAlert;
    }
}).call(function() {
    return this || (typeof window !== 'undefined' ? window : global);
}());