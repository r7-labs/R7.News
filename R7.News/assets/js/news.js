var r7_news = r7_news || {};

r7_news.service = function ($, moduleId) {
    var baseServicePath = $.dnnSF (moduleId).getServiceRoot ("R7.News");
    this.ajaxCall = function (type, controller, action, id, data, success, fail) {
        $.ajax ({
            type: type,
            url: baseServicePath + controller + "/" + action + (id != null ? "/" + id : ""),
            beforeSend: $.dnnSF (moduleId).setModuleHeaders,
            data: data
        }).done (function (retData) {
            if (success != undefined) {
                success (retData);
            }
        }).fail (function (xhr, status) {
            if (fail != undefined) {
                fail (xhr, status);
            }
        });
    };

    this.getNewsEntryText = function (success, fail, data) {
        this.ajaxCall ("GET", "News", "GetNewsEntryText", null, data, success, fail);
    };
}

r7_news.loadMoreText = function (btn, entryTextId, moduleId) {
	var service = new r7_news.service ($, moduleId);
	service.getNewsEntryText (
		function (data) {
			$(btn).parent().append(data.rawText);
            $(btn).hide();
		},
		function (xhr, status) {
            console.error ("R7.News: Error loading more text!", xhr);
            $(btn).parent().append('<p class="text-danger">Error loading more text!</p>');
            $(btn).hide();
		},
		{ entryTextId: entryTextId }
	);
}
