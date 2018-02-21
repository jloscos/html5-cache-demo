function onUpdateReady() {
	location.reload(true);
}

window.applicationCache.addEventListener('updateready', onUpdateReady);
if (window.applicationCache.status === window.applicationCache.UPDATEREADY) {
	onUpdateReady();
}
window.applicationCache.addEventListener('error', onUpdateReady);

$(".article-title").click(function (e) {
	e.stopPropagation();
	e.preventDefault();
	var container = $(this).parent().find(".content");
	$(this).parent().toggleClass("expanded");
	if (container.children().length == 0) {
		$.ajax("/Feed/ArticleContent?ArticleId=" + $(this).data("id"), {
			success: function (data) {
				container.html(data);
			}
		});
	}
});