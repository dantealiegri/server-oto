
       $.ajaxSetup({ cache: false, dataType: 'json' });

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

        function _mkAjax(url, data, callback, type, method) {
            if ($.isFunction(data)) {
                callback = data, data = {};
            }
            return $.ajax({ type: method, url: url, data: data, success: callback, contentType: type });
        }

        $.extend({
            put: function (url, data, callback, type) {
                return _mkAjax(url, data, callback, type, 'PUT');
            },
            del: function (url, data, callback, type) {
                return _mkAjax(url, data, callback, type, 'DELETE');
            }
        });

        var href = "files", lastHref = "";

        var mkdir = function () {
            var dir = $("#dirname");
            if (!dir.val()) {
                alert("Enter the name of the folder first");
                dir.focus();
                return;
            }
            $.post(href + "/" + dir.val(), null, function () { dir.val(''); refresh(); });
        };

        $(".mkdir .btn").click(mkdir);
        $(".mkdir INPUT").keypress(function (e) { if (e.which == '13') mkdir(); });
        $(".upload .btn").click(function () {
            var file = $("#fileupload").val();
            if (!file) {
                alert("Select a file to upload first");
                return;
            }
            try {
                $.ajaxFileUpload({
                    url: href,
                    secureuri: false,
                    fileElementId: 'fileupload',
                    success: refresh
                });
            }
            catch (e) {
                alert("Not supported in " + ($.browser.msie ? "IE" : "this browser"));
            }
        });

        $(".btn").mousedown(function () { $(this).toggleClass("mousedown"); });
        $(".btn").mouseup(function () { $(this).toggleClass("mousedown"); });



            if (!skipPushState && window.history.pushState)
                window.history.pushState(href, href.replace('/', ' / '), '#!' + href);

            var dirs = href.replace(/\/$/, "").split('/');
            var sb = '<div id="breadcrumb">';
            var sbDirs = "";
            for (var i = 0; i < dirs.length; i++) {
                var dir = dirs[i];
                if (!dir) continue;
                sb += (i == dirs.length - 1)
                ? '<strong>' + dir + '</strong>'
                : '<a href="#!' + sbDirs + dir + '">' + dir + '</a><b>/</b>';
                sbDirs += dir + "/";
            }
            $("#breadcrumb").html(sb + "</div>");

            var jqLs = $("#ls");
            $.getJSON(href, function (r) {
                var navBack = lastHref.length > href.length && lastHref.substr(0, href.length) == href,
                nextCls = navBack ? "results-0" : "results-2",
                hasResults = $("#ls TABLE").length == 1,
                cls = !hasResults ? "results-1" : nextCls;
                var sb = "<div class='" + cls + "'><table><thead><tr><th>name</th><th>age</th><th>size</th></thead><tbody>";

                var file = r.Files;
                if (file) {

                    var jqFile = $("#fileview");
                    var sb = "<h3><a class='btn edit' href='#!savechanges'><span>save changes</span></a></dd>"
                       + "<span class='ib txt'></span><dl><dd>" + enc(file.FileSizeBytes) + " bytes</dd><dd>"
                       + toTwitterTime(fromDtoDate(file.ModifiedDate)) + "</dd>"
                       + '<dd><a class="btn download" href="' + href + '?ForDownload=true"><span>download file</span></a></dd>'
                       + "</dl></h3>";
                    jqFile.html(sb).show();

                    var height = $("#footer").position().top - $("#ls").position().top;
                    $("#fileview TEXTAREA").height(height - 65);

                    $("#fileadmin").hide();
                    $("#ls").html("").hide();
                    return;
                }

                $("#fileview").html("").hide();
                var dirList = r.Directory;
                if (dirList) {
                    if (dirs.length > 1) {
                        var upHref = href.substr(0, href.lastIndexOf('/', href.length - 2));
                        sb += "<tr><td><a class='up-dir' href='#!" + upHref + "'>..<a></td><td></td><td></td></tr>";
                    }
                    $.each(dirList.Folders, function (i, dir) {
                        sb += "<tr><td><a class='dir' href='#!" + href + "/" + dir.Name + "'>" + dir.Name + "/<a></td><td>"
                        + toTwitterTime(fromDtoDate(dir.ModifiedDate)) + "</td><td>"
                        + dir.FileCount + " files</td>";
                    });
                    $.each(dirList.Files, function (i, file) {
                        sb += "<tr><td><a class='file' href='#!" + href + "/" + file.Name + "'><b class='del' href='#!deletefile'></b>" + file.Name + "</a></td><td>"
                        + toTwitterTime(fromDtoDate(file.ModifiedDate)) + "</td><td>"
                        + file.FileSizeBytes + " bytes</td>";
                    });
                }

                sb += "</tbody></table></div>";

                $("#fileadmin").show();
                $("#ls").show().append(sb);

                var jq1 = $(".results-1"), jq2 = $("." + nextCls), el1 = jq1[0], el2 = jq2[0];
                if (el1 && el2) {
                    jqLs.css({ "min-height": Math.max(jq1.height(), jq2.height()) + "px" });

                    jq1.addClass(navBack ? "slide-right" : "slide-left");
                    jq2.addClass(navBack ? "slide-right" : "slide-left");

                    setTimeout(function () {
                        jqLs.children().first().remove();
                        jqLs.children().first()[0].className = "results-1";
                    }, 450);
                }
                else {
                    $("#ls").css({ "min-height": jq1.height() + "px" });
                }
            });