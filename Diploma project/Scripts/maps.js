$(document).ready(function () {
    google.maps.visualRefresh = true;
    var Bam = new google.maps.LatLng(53.9077013, 27.5864255);
    var mapOptions = {
        zoom: 14,
        center: Bam,
        mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
    };
    var map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
    var data = [{
        "Id": 1, "PlaceName": "ОАО «БелЭлектроМонтаж»",
        "Adress": "Улица Берестянская 12, Минск, Беларусь", "TimeWork": "9:00 - 18:00",
        "Phone": "+375 (17) 303-90-22", "Phacs": "+375 (17) 356-07-45",
        "GeoLong": "53.9077013", "GeoLat": "27.5864255"
    }];
    $.each(data, function (i, item) {
        var marker = new google.maps.Marker({
            'position': new google.maps.LatLng(item.GeoLong, item.GeoLat),
            'map': map,'title': item.PlaceName});
        marker.setIcon('http://maps.google.com/mapfiles/ms/icons/blue-dot.png');
        var infowindow = new google.maps.InfoWindow({
            content: "<div class='infoDiv'><h5>" + item.PlaceName +
                "</h5><hr />" + "<div><p>Адрес: <i>" + item.Adress +
                "</i></p><p>Время работы: <i>" + item.TimeWork + "</i></p><p>Телефон: <i>" +
                item.Phone + "</i></p><p>Факс: <i>" + item.Phacs + "</i></p></div></div>"});
        google.maps.event.addListener(marker, 'click', function () { infowindow.open(map, marker); });
    })
});
