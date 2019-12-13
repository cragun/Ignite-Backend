var validations = {
    isiPad: navigator.userAgent.match(/iPad/i) != null,
    isiPhone: (navigator.userAgent.match(/iPhone/i)) || (navigator.userAgent.match(/iPod/i)) || false
};

document.addEventListener('DOMContentLoaded', function () {
    // define readURL function which will in time live preview the selected image
    var readURL = function (input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                let previewImage = document.querySelector('.image-preview');
                if (previewImage) {
                    previewImage.src = e.target.result;
                }
            }

            reader.readAsDataURL(input.files[0]);
        }
    };

    // add input event listener to mask phone number input
    document.querySelector('#PhoneNumber').addEventListener('input', function (event) {
        var x = event.target.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
        event.target.value = !x[2] ? x[1] : x[1] + '-' + x[2] + (x[3] ? '-' + x[3] : '');
    });

    // add change event listener on the image
    let profileImage = document.querySelector('#profilePicture');
    if (profileImage) {
        profileImage.addEventListener('change', function () {
            readURL(this);
        });
    }

    // show open on iDevice button
    var iDeviceOnlyElements = document.querySelectorAll('.is-idevice')
    if (iDeviceOnlyElements) {
        var visibility = validations.isiPad || validations.isiPhone ? 'block' : 'none';
        for (var i = 0; i < iDeviceOnlyElements.length; i++) {
            iDeviceOnlyElements[i].style.display = visibility;
        }
    }
});

function openOniDevice() {
    if (!redirectUrl) {
        return;
    }
    location.href = redirectUrl;
}
