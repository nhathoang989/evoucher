(function ($) {
    window.Common = {
        getBase64: async function (file, container) {
            if (file !== null) {
                var reader = new FileReader();
                reader.readAsDataURL(file);
                reader.onload = function () {
                    var index = reader.result.indexOf(',') + 1;
                    var base64 = reader.result.substring(index);
                    if (container) {
                        container.val(reader.result);
                    }
                    return base64;
                };
                reader.onerror = function (error) {
                    console.log(error);
                };
            }
            else {
                return null;
            }
        },
    }
}(jQuery));