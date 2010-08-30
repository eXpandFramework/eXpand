var mycarousel_itemList = [];
function mycarousel_itemLoadCallback(carousel, state) {

    id = jQuery("UL", carousel.container[0])[0].id;
    for (var i = carousel.first; i <= carousel.last; i++) {
        if (carousel.has(i)) {
            continue;
        }

        if (i > mycarousel_itemList[id].length) {
            break;
        }
        var item = mycarousel_itemList[id][i - 1];
        html = '';
        if (item.url+''!='')
            html = '<img src="' + item.url + '" alt="' + item.alt + '" /><br>';
        html +=  item.text;
        carousel.add(i, html);
    }
};