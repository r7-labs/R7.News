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

r7_news.expandText = function (btn, entryTextId, moduleId) {
	var service = new r7_news.service ($, moduleId);
	service.getNewsEntryText (
		function (data) {
			$(btn).parent().parent().prev().append(data.rawText);
            $(btn).parent().hide();
		},
		function (xhr, status) {
            console.error ("R7.News: Error loading text!", xhr);
            // TODO: Localize error message
            $(btn).parent().parent().prev().append('<p class="text-danger">Error loading text! You can try to reload the page and try again.</p>');
            $(btn).parent().hide();
		},
		{ entryTextId: entryTextId }
	);
}
