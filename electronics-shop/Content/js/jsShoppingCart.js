
$(document).ready(function () {
    showCount();
  
    $('body').on('click', '.btnAddToCart', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var quatity = 1;
        var tQuantity = $('#quantity').val();
        if (tQuantity != '') {
            quatity = parseInt(tQuantity);
        }

        alert(id + " " + tQuantity);

        $.ajax({
            url: '/ShoppingCart/AddToCart',
            type: 'POST',
            data: { id: id, quantity: quatity },
            success: function (rs) {
                if (rs.Success ) {
                    $('#checkout_items').html(rs.Count);


                    alert(rs.msg);
                } else {
                    openLogin();
                    alert(rs.msg);
                 
                    
                }
            }
        });  

    
    });

    $('body').on('click', '.btnUpdate', function (e) {
        e.preventDefault();
        var id = $(this).data("id");
        var quantity = $('#Quantity_' + id).val();
        Update(id, quantity);
        alert('thay đổi thành công');
    });

    // wishlist
    $('body').on('click', '.btn-wishlist', function (e) {
        e.preventDefault();
        var id = $(this).data("id");
      
            $.ajax({
                url: '/WishList/AddToWishList',
                type: 'POST',
                data: { id: id },
                success: function (rs) {
                    if (rs.Success) {
                        $('#checkout_wishs').html(rs.Count);


                        alert(rs.msg);
                        alert('thêm vào wishlist thành công' + id);
                    }
                }
            });
        });
     
      


    $('body').on('click', '.btnOpenWish', function (e) {
        e.preventDefault();
        $.ajax({
            url: '/WishList/AddToWishList',
            type: 'GET',
            success: function (partialView) {

                $('#wish').html(partialView);

            },

            error: function () {
                console.log('Lỗi khi tải lại PartialView');
            }



        });
    });
    $('body').on('click', '.btnDeleteAll', function (e) {
        e.preventDefault();
        var conf = confirm('Bạn có chắc muốn xóa toàn bộ giỏ hàng?');
        if (conf == true) {
            DeleteAll();
            alert("xóa thành công");
        }
    });
    $('body').on('click', '.btn-open-modal', function (e) {
        e.preventDefault();
        reset();
        resetTotal();   
    });
    $('body').on('click', '.btn_Delete', function (e) {
        e.preventDefault()
        var id = $(this).data('id');
        var conf = confirm('Bạn có chắc muốn xóa sản phầm này khỏi giỏ hàng?');
        if (conf == true) {
            $.ajax({
                url: '/ShoppingCart/Delete',
                type: 'POST',
                data: { id: id },
                success: function (rs) {
                    if (rs.Success) {
                        $('#checkout_items').html(rs.Count);
                        $('#trow_' + id).remove();

              
                        LoadCart();
                        reset();
                        resetTotal();

                    }

                }

            });

        }
    });

});


function openLogin(){
    $.ajax({
        url: '/Account/Login',
        type: 'GET',
        success: function (response) {

            var modal = $('#login').html(response).modal();
            modal.on('hidden.bs.modal', function(){
                modal.remove();
            });
            alert("login");

        }




    });
}


function resetTotal(){
    $.ajax({
        url: '/Products/RenderActionCart',
        type: 'GET',
        success: function (partialView) {
           
            $('#partialViewAction').html(partialView);
            
        },

        error: function () {
            console.log('Lỗi khi tải lại PartialView');
        }



    });
}
function reset() {
    $.ajax({
        url: '/Products/RenderShoppingCart',
        type: 'GET',
        success: function (partialView) {

            $('#partialViewContainer').html(partialView);
            
        },

        error: function () {
            console.log('Lỗi khi tải lại PartialView');
        }



    });
}


// Delete cart





function showCount() {
    $.ajax({
        url: '/ShoppingCart/ShowCount',
        type: 'GET',
        success: function (rs) {
                $('#checkout_items').html(rs.Count);
        }
    });
    $.ajax({
        url: '/Product/Shop',
        type: 'GET',
        success: function (rs) {
            $('#checkout_items').html(rs.Count);
        }
    });
    $.ajax({
        url: '/Product/Details',
        type: 'GET',
        success: function (rs) {
            $('#checkout_items').html(rs.Count);
        }
    });
    $('body').on('click', '.btnAddToCart', function (e) {
        e.preventDefault();
        var id = $(this).data('id');


        $.ajax({

            url: '/Product/Details/id',
            type: 'GET',
            success: function (rs) {
                $('#checkout_items').html(rs.Count);
            }
        });

    });
    $.ajax({
        url: '/ShoppingCart/Index',
        type: 'GET',
        success: function (rs) {
            $('#checkout_items').html(rs.Count);
        }
    });

}

//show wishlist cart
function showCountList() {
    $.ajax({
        url: '/WishList/ShowCount',
        type: 'GET',
        success: function (rs) {
            $('#checkout_wishs').html(rs.Count);
        }
    });
    $.ajax({
        url: '/Product/Shop',
        type: 'GET',
        success: function (rs) {
            $('#checkout_wishs').html(rs.Count);
        }
    });
    $.ajax({
        url: '/Product/Details',
        type: 'GET',
        success: function (rs) {
            $('#checkout_wishs').html(rs.Count);
        }
    });
    $('body').on('click', '.btnAddToCart', function (e) {
        e.preventDefault();
        var id = $(this).data('id');


        $.ajax({

            url: '/Product/Details/id',
            type: 'GET',
            success: function (rs) {
                $('#checkout_wishs').html(rs.Count);
            }
        });

    });
    $.ajax({
        url: '/ShoppingCart/Index',
        type: 'GET',
        success: function (rs) {
            $('#checkout_wishs').html(rs.Count);
        }
    });

}
//delete all
function DeleteAll() {
    $.ajax({
        url: '/ShoppingCart/DeleteAll',
        type: 'POST',
        success: function (rs) {
            if (rs.Success) {
                LoadCart();
                showCount();
            }
        }
    });
}

//update
function Update(id,quantity) {
    $.ajax({
        url: '/ShoppingCart/Update',
        type: 'POST',
        data: { id: id, quantity:quantity },
        success: function (rs) {
            if (rs.Success) {
                LoadCart();
                showCount();
            }
        }
    });
}


// load cart

function LoadCart() {
    $.ajax({
        url: '/ShoppingCart/Partial_Item_Cart',
        type: 'GET',
        success: function (rs) {
            
                $('#load_data').html(rs);
            
        }
    });
}

