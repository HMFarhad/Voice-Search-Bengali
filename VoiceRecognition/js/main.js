(function($) {
	
    "use strict";
    //Dropdown Menu
    $(document).ready(function(){
        $("nav > ul > li.has-dropdown > a").click(function(){
          $("nav > ul > li > ul").slideToggle();
          $(this).toggleClass('ncons');
        });
        //Mobile
        $(".menu-btn > a").click(function(){
            $(".menu-list").slideToggle();
            $(this).toggleClass('micons');
        });
        // Login
        $(".login > a").click(function(){
            $(".login-wrap").slideToggle();
        });
        // Login
        $(".logs > a.login-close").click(function(){
            $(".login-wrap").hide();
        });
    });
    
  
    // Slider 
    $('.owl-carousel').owlCarousel({
        items:1,
        lazyLoad:true,
        loop:true,
        animateOut: 'fadeOut',
        animateIn: 'fadeIn'
    });
    	//Tabs Box
        if($('.tabs-box').length){
            $('.tabs-box .tab-buttons .tab-btn').on('click', function(e) {
                e.preventDefault();
                var target = $($(this).attr('data-tab'));
                
                if ($(target).is(':visible')){
                    return false;
                }else{
                    target.parents('.tabs-box').find('.tab-buttons').find('.tab-btn').removeClass('active-btn');
                    $(this).addClass('active-btn');
                    target.parents('.tabs-box').find('.tab-container').find('.tab-content').fadeOut(0);
                    target.parents('.tabs-box').find('.tab-container').find('.tab-content').removeClass('active-tab animated fadeIn');
                    $(target).fadeIn(300);
                    $(target).addClass('active-tab animated fadeIn');
                }
            });
        }
        if($('.itabs-box').length){
            $('.itabs-box .itab-buttons .itab-btn').on('click', function(e) {
                e.preventDefault();
                var target = $($(this).attr('data-tab'));
                
                if ($(target).is(':visible')){
                    return false;
                }else{
                    target.parents('.itabs-box').find('.itab-buttons').find('.itab-btn').removeClass('active-ibtn');
                    $(this).addClass('active-ibtn');
                    target.parents('.itabs-box').find('.itab-container').find('.itab-content').fadeOut(0);
                    target.parents('.itabs-box').find('.itab-container').find('.itab-content').removeClass('active-itab animated fadeIn');
                    $(target).fadeIn(300);
                    $(target).addClass('active-itab animated fadeIn');
                }
            });
        }
 


        $(document).ready(function(){
            $('.cbtn1').click(function(e){
                var button_classes, value = +$('.counter1').val();
                button_classes = $(e.currentTarget).prop('class');        
                if(button_classes.indexOf('up_count1') !== -1){
                    value = (value) + 1;            
                } else {
                    value = (value) - 1;            
                }
                value = value < 0 ? 0 : value;
                $('.counter1').val(value);
            });  
            $('.counter1').click(function(){
                $(this).focus().select();
            });

            $('.cbtn2').click(function(e){
                var button_classes, value = +$('.counter2').val();
                button_classes = $(e.currentTarget).prop('class');        
                if(button_classes.indexOf('up_count2') !== -1){
                    value = (value) + 1;            
                } else {
                    value = (value) - 1;            
                }
                value = value < 0 ? 0 : value;
                $('.counter2').val(value);
            });  
            $('.counter2').click(function(){
                $(this).focus().select();
            });
            $('.cbtn3').click(function(e){
                var button_classes, value = +$('.counter3').val();
                button_classes = $(e.currentTarget).prop('class');        
                if(button_classes.indexOf('up_count3') !== -1){
                    value = (value) + 1;            
                } else {
                    value = (value) - 1;            
                }
                value = value < 0 ? 0 : value;
                $('.counter3').val(value);
            });  
            $('.counter3').click(function(){
                $(this).focus().select();
            });
        });

        $(document).ready(function(){
            $(".travelers").click(function(){
              $(".travelers-counter").slideToggle();
              $(this).toggleClass('icons');
            });
        });

      
})(jQuery);  
