$(document).ready(function ($) {

    ////team two mesonary 	
    ////=================
    //jQuery(function () {
    //    // now doc is ready, make selection
    //    // use another selector, not .isotope,
    //    // since that is dynamically added in Isotope v1
    //    var $container = jQuery('#list .row');
    //    // use imagesLoaded, instead of window.load
    //    $container.imagesLoaded(function () {
    //        $container.masonry({ itemSelector: '.item' });
    //    });
    //});

    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('.header-area').addClass('sticky')
        } else {
            $('.header-area').removeClass('sticky')
        }
    });

    /*<!-------------------- partner logo ( .owlCarousel )----------------------------------->*/

    $('.partner-logo').owlCarousel({

        autoPlay: true,
        slideSpeed: 1000,
        pagination: false,
        navigation: false,
        items: 5,
        navigationText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"],
        itemsDesktop: [1199, 4],
        itemsDesktopSmall: [980, 3],
        itemsTablet: [768, 2],
        itemsMobile: [479, 2],
    }); 

    
    


       /*<!---------------feature slide //----------------------------------->*/

                    
        $(".feature-slide").owlCarousel({
            autoPlay: false,
            slideSpeed: 1000,
            pagination: false,
            navigation: false,
            items: 4,
            navigationText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"],
            itemsDesktop: [1199, 4],
            itemsDesktopSmall: [980, 3],
            itemsTablet: [768, 1],
            itemsMobile: [479, 1],
        });
    

    /*<!------------------- feature slide -----------------------------------*/

    $(".testimonial-slide").owlCarousel({
        autoPlay: true,
        slideSpeed: 1000,
        pagination: false,
        navigation: false,
        items: 3,
        navigationText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"],
        itemsDesktop: [1199, 3],
        itemsDesktopSmall: [980, 2],
        itemsTablet: [768, 1],
        itemsMobile: [479, 1],
    });
    
    $(".testimonial-slide2").owlCarousel({
        autoPlay: true,
        slideSpeed: 1000,
        pagination: false,
        navigation: false,
        items: 1,
        navigationText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"],
        itemsDesktop: [1199, 2],
        itemsDesktopSmall: [980, 2],
        itemsTablet: [768, 1],
        itemsMobile: [479, 1],
    });

    /*---------------------------- team slide -----------------------------------*/ 
    $(".team-member-slide").owlCarousel({
        autoPlay: true,
        slideSpeed: 1000,
        pagination: false,
        navigation: true,
        items: 4,
        navigationText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"],
        itemsDesktop: [1199, 4],
        itemsDesktopSmall: [980, 2],
        itemsTablet: [768, 1],
        itemsMobile: [479, 1],
    });

    var owl = $('.owl-carousel');
        owl.owlCarousel({
            margin: 10,
            nav: true,
            loop: true,
            responsive: {
                0: {
                    items: 1
                },
                600: {
                    items: 3
                },
                1000: {
                    items: 5
                }
            }
        });
    
});

//$(document).ready(function () {

//    var owl = $('.owl-carousel');
//        owl.owlCarousel({
//            margin: 10,
//            nav: true,
//            loop: true,
//            responsive: {
//                0: {
//                    items: 1
//                },
//                600: {
//                    items: 3
//                },
//                1000: {
//                    items: 5
//                }
//            }
//        });


//});