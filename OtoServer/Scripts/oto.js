var app = angular.module('oto' , ['restangular']).
  config(function($routeProvider, RestangularProvider) {
	  $routeProvider.
	  when('/', { controller:AppCtrl,templateUrl:'app.html' }).
	  otherwise({redirectTo:'/'});
	  RestangularProvider.setBaseUrl("/oto");
	  RestangularProvider.setRestangularFields({
		  id : "LinkName"
	  });
	  RestangularProvider.setResponseExtractor(function(response,operation) {
		  var newResponse;
		  newResponse = response.Containers;
		  newResponse.trail = { names : response.breadcrumbnames , links :response.breadcrumbs }
		  console.log( "Response: " + JSON.stringify(response));
		  console.log( "Operation:" + operation);
		  console.log( "Modified: " + JSON.stringify(newResponse))
		  console.log( "Modified Trail: " + JSON.stringify(newResponse.trail))
		  return newResponse;
	  });
  });



function AppCtrl($scope,Restangular) {
       	$scope.oto_data = Restangular.all("files").getList();
	$scope.oto_data.then(function(files) {
		console.log("Files Loaded: " +  JSON.stringify(files));
	});
}


function fromDtoDate(dateStr) {
	return new Date(parseFloat(/Date\(([^)]+)\)/.exec(dateStr)[1]));
}

function toTwitterTime(a) {
	var b = new Date();
	var c = typeof a == "date" ? a : new Date(a);
	var d = b - c;
	var e = 1000, minute = e * 60, hour = minute * 60, day = hour * 24, week = day * 7;
	if (isNaN(d) || d < 0) { return "" }
	if (d < e * 7) { return "right now" }
	if (d < minute) { return Math.floor(d / e) + " secs ago" }
	if (d < minute * 2) { return "about 1 min ago" }
	if (d < hour) { return Math.floor(d / minute) + " mins ago" }
	if (d < hour * 2) { return "about 1 hour ago" }
	if (d < day) { return Math.floor(d / hour) + " hours ago" }
	if (d > day && d < day * 2) { return "yesterday" }
	if (d < day * 365) { return Math.floor(d / day) + " days ago" } else { return "over a year ago" }
}

function enc(html) {
	if (typeof html != "string") return html;
	return html.replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

function dirPath(path) {
	if (typeof path != "string") return path;
	var strPos = path.lastIndexOf('/', path.length - 1);
	if (strPos == -1) return path;
	return path.substr(0, strPos);
}

