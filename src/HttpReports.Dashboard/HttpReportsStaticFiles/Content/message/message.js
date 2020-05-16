
/**
 * 消息提示组件
 * 
 * 1.调用
 * 字符串类型参数： $.message('成功');
 * 对象型参数：$.message({});
 * 
 * 2.参数详解
 *  message:' 操作成功',    //提示信息
    time:'2000',           //显示时间（默认：2s）
    type:'success',        //显示类型，包括4种：success.error,info,warning
    showClose:false,       //显示关闭按钮（默认：否）
    autoClose:true,        //是否自动关闭（默认：是）
 * 
 * type:success,error,info,warning
 */

$.extend({
  message: function(options) {
      var defaults={
          message:' 操作成功',
          time:'2000',
          type:'success',
          showClose:false,
          autoClose:true,
          onClose:function(){}
      };
      
      if(typeof options === 'string'){
          defaults.message=options;
      }
      if(typeof options === 'object'){
          defaults=$.extend({},defaults,options);
      }
      //message模版
      var templateClose=defaults.showClose?'<a class="c-message--close">×</a>':'';
      var template='<div class="c-message messageFadeInDown">'+
          '<div class="c-message--main">' +
            '<i class=" c-message--icon c-message--'+defaults.type+'"></i>'+
            templateClose+
            '<div class="c-message--tip">'+defaults.message+'</div>'+
          '</div>'+
      '</div>';
      var _this=this;
      var $body=$('body');
      var $message=$(template);
      var timer;
      var closeFn,removeFn;
      //关闭
      closeFn=function(){
          $message.addClass('messageFadeOutUp');
          $message.one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend',function(){
              removeFn();
          })
      };
      //移除
      removeFn=function(){
          $message.remove();
          defaults.onClose(defaults);
          clearTimeout(timer);
      };
      //移除所有
      $('.c-message').remove();
      $body.append($message);
      //居中
      $message.css({
          'margin-left':'-'+$message.width()/2+'px'
      })
      //去除动画类
      $message.one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend',function(){
          $message.removeClass('messageFadeInDown');
      });
      //点击关闭
      $body.on('click','.c-message--close',function(e){
          closeFn();
      });
      //自动关闭
      if(defaults.autoClose){
          timer=setTimeout(function(){
              closeFn();
          },defaults.time)
      }
  }
});