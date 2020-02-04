/**
 * Flot plugin that provides spline interpolation for line graphs
 * author: Alex Bardas < alex.bardas@gmail.com >
 * modified by: Avi Kohn https://github.com/AMKohn
 * based on the spline interpolation described at:
 *		 http://scaledinnovation.com/analytics/splines/aboutSplines.html
 *
 * Example usage: (add in plot options series object)
 *		for linespline:
 *			series: {
 *				...
 *				lines: {
 *					show: false
 *				},
 *				splines: {
 *					show: true,
 *					tension: x, (float between 0 and 1, defaults to 0.5),
 *					lineWidth: y (number, defaults to 2),
 *					fill: z (float between 0 .. 1 or false, as in flot documentation)
 *				},
 *				...
 *			}
 *		areaspline:
 *			series: {
 *				...
 *				lines: {
 *					show: true,
 *					lineWidth: 0, (line drawing will not execute)
 *					fill: x, (float between 0 .. 1, as in flot documentation)
 *					...
 *				},
 *				splines: {
 *					show: true,
 *					tension: 0.5 (float between 0 and 1)
 *				},
 *				...
 *			}
 *
 */(function(e){"use strict";function t(e,t,n,r,i,s,o){var u=Math.pow,a=Math.sqrt,f,l,c,h,p,d,v,m;f=a(u(n-e,2)+u(r-t,2));l=a(u(i-n,2)+u(s-r,2));c=o*f/(f+l);h=o-c;p=n+c*(e-i);d=r+c*(t-s);v=n-h*(e-i);m=r-h*(t-s);return[p,d,v,m]}function r(t,n,r,i,s){var o=e.color.parse(s);o.a=typeof i=="number"?i:.3;o.normalize();o=o.toString();n.beginPath();n.moveTo(t[0][0],t[0][1]);var u=t.length;for(var a=0;a<u;a++)n[t[a][3]].apply(n,t[a][2]);n.stroke();n.lineWidth=0;n.lineTo(t[u-1][0],r);n.lineTo(t[0][0],r);n.closePath();if(i!==!1){n.fillStyle=o;n.fill()}}function i(e,t,r,i){if(t===void 0||t!=="bezier"&&t!=="quadratic")t="quadratic";t+="CurveTo";if(n.length==0)n.push([r[0],r[1],i.concat(r.slice(2)),t]);else if(t=="quadraticCurveTo"&&r.length==2){i=i.slice(0,2).concat(r);n.push([r[0],r[1],i,t])}else n.push([r[2],r[3],i.concat(r.slice(2)),t])}function s(s,o,u){if(u.splines.show!==!0)return;var a=[],f=u.splines.tension||.5,l,c,h,p=u.datapoints.points,d=u.datapoints.pointsize,v=s.getPlotOffset(),m=p.length,g=[];n=[];if(m/d<4){e.extend(u.lines,u.splines);return}for(l=0;l<m;l+=d){c=p[l];h=p[l+1];if(c==null||c<u.xaxis.min||c>u.xaxis.max||h<u.yaxis.min||h>u.yaxis.max)continue;g.push(u.xaxis.p2c(c)+v.left,u.yaxis.p2c(h)+v.top)}m=g.length;for(l=0;l<m-2;l+=2)a=a.concat(t.apply(this,g.slice(l,l+6).concat([f])));o.save();o.strokeStyle=u.color;o.lineWidth=u.splines.lineWidth;i(o,"quadratic",g.slice(0,4),a.slice(0,2));for(l=2;l<m-3;l+=2)i(o,"bezier",g.slice(l,l+4),a.slice(2*l-2,2*l+2));i(o,"quadratic",g.slice(m-2,m),[a[2*m-10],a[2*m-9],g[m-4],g[m-3]]);r(n,o,s.height()+10,u.splines.fill,u.color);o.restore()}var n=[];e.plot.plugins.push({init:function(e){e.hooks.drawSeries.push(s)},options:{series:{splines:{show:!1,lineWidth:2,tension:.5,fill:!1}}},name:"spline",version:"0.8.2"})})(jQuery);