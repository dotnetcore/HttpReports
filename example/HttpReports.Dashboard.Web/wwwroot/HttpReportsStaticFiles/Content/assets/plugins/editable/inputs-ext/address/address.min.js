/**
Address editable input.
Internally value stored as {city: "Moscow", street: "Lenina", building: "15"}

@class address
@extends abstractinput
@final
@example
<a href="#" id="address" data-type="address" data-pk="1">awesome</a>
<script>
$(function(){
    $('#address').editable({
        url: '/post',
        title: 'Enter city, street and building #',
        value: {
            city: "Moscow", 
            street: "Lenina", 
            building: "15"
        }
    });
});
</script>
**/(function(e){"use strict";var t=function(e){this.init("address",e,t.defaults)};e.fn.editableutils.inherit(t,e.fn.editabletypes.abstractinput);e.extend(t.prototype,{render:function(){this.$input=this.$tpl.find("input")},value2html:function(t,n){if(!t){e(n).empty();return}var r=e("<div>").text(t.city).html()+", "+e("<div>").text(t.street).html()+" st., bld. "+e("<div>").text(t.building).html();e(n).html(r)},html2value:function(e){return null},value2str:function(e){var t="";if(e)for(var n in e)t=t+n+":"+e[n]+";";return t},str2value:function(e){return e},value2input:function(e){if(!e)return;this.$input.filter('[name="city"]').val(e.city);this.$input.filter('[name="street"]').val(e.street);this.$input.filter('[name="building"]').val(e.building)},input2value:function(){return{city:this.$input.filter('[name="city"]').val(),street:this.$input.filter('[name="street"]').val(),building:this.$input.filter('[name="building"]').val()}},activate:function(){this.$input.filter('[name="city"]').focus()},autosubmit:function(){this.$input.keydown(function(t){t.which===13&&e(this).closest("form").submit()})}});t.defaults=e.extend({},e.fn.editabletypes.abstractinput.defaults,{tpl:'<div class="editable-address"><label><span>City: </span><input type="text" name="city" class="input-small"></label></div><div class="editable-address"><label><span>Street: </span><input type="text" name="street" class="input-small"></label></div><div class="editable-address"><label><span>Building: </span><input type="text" name="building" class="input-mini"></label></div>',inputclass:""});e.fn.editabletypes.address=t})(window.jQuery);