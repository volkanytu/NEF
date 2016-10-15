var Lock = function () {

    return {
        //main function to initiate the module
        init: function () {

             $.backstretch([
		       "images/1.jpg",
    		    "images/2.jpg",
    		    "images/3.jpg",
    		    "images/4.jpg"
		        ], {
		          fade: 1000,
		          duration: 8000
		      });
        }

    };

}();