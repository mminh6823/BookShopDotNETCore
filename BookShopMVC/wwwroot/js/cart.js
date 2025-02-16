const itemQuantityInputs = document.querySelectorAll('.cart-item-quantity-input')
const itemDeleteButtons = document.querySelectorAll('.cart-item-remove-btn')

itemQuantityInputs.forEach(input => {
    input.addEventListener('change', () => {
        const productId = parseInt(input.getAttribute('data-id'))
        const quantity = parseInt(input.value)

        fetch(`/api/user/cart/item/${productId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: quantity
        })
            .then(res => {
                if (!res.ok) {
                    return;
                }
                return res.json()
            })
            .then(data => {
                const toast = {
                    title: "Thành công",
                    message: "Cập nhật số lượng thành công.",
                    status: TOAST_STATUS.SUCCESS,
                    timeout: 2000
                }

                if (!data || data.status != "success") {
                    toast.title = "Lỗi"
                    toast.message = "Gặp lỗi khi cập nhật số lượng."
                    toast.status = TOAST_STATUS.DANGER
                }
                Toast.create(toast);
                refreshItemQuantity(productId)
                refreshProductPrice(productId, data.data.items)
                updateTotalPrices(data.data)
            })
    })
})

itemDeleteButtons.forEach(button => {
    button.addEventListener('click', () => {
        const productId = parseInt(button.getAttribute('data-id'));

        fetch(`/api/user/cart/item/${productId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(res => res.json())
            .then(data => {
                let toast = {
                    title: "Thành công",
                    message: "Đã xóa sản phẩm.",
                    status: TOAST_STATUS.SUCCESS,
                    timeout: 2000
                };

                if (!data || data.status !== "success") {
                    toast = {
                        title: "Lỗi",
                        message: "Đã xảy ra lỗi khi xóa sản phẩm.",
                        status: TOAST_STATUS.DANGER,
                        timeout: 2000
                    };
                }

                Toast.create(toast);

                if (data.status === "success") {
                    removeCartItem(productId);
                    refreshCartProductAmount(data.data.itemsQuantity);
                    updateTotalPrices(data.data);

                    // Kiểm tra nếu giỏ hàng trống
                    if (data.data.itemsQuantity === 0) {
                        displayEmptyCart();
                    }
                }
            })
            .catch(() => {
                Toast.create({
                    title: "Lỗi",
                    message: "Mất kết nối, vui lòng thử lại!",
                    status: TOAST_STATUS.DANGER,
                    timeout: 2000
                });
            });
    });
});


function removeCartItem(productId) {
    const item = document.querySelector(`#cart-item-${productId}`)
    item.remove()
}

function refreshItemQuantity(productId) {
    const itemQuantityInput = document.querySelector(`#quantity-${productId}-input`)
    const itemQuantity = document.querySelector(`#quantity-${productId}`)
    itemQuantity.textContent = itemQuantityInput.value
}

function refreshProductPrice(productId, cartItems) {
    const itemTotalPrice = document.querySelector(`#total-price-${productId}`)
    const cartItem = cartItems.find(item => item.productId == productId)
    itemTotalPrice.textContent = cartItem.totalPrice.formatted
}

function updateTotalPrices(cart) {
    document.querySelector('#total').textContent = cart.total.formatted;
    document.querySelector('#subtotal').textContent = cart.subtotal.formatted;
    document.querySelector('#vat').textContent = cart.vat.formatted;
    document.querySelector('#shipping').textContent = cart.shipping.formatted;
}


// Hiển thị giỏ hàng trống nếu không còn sản phẩm
function displayEmptyCart() {
    document.querySelector('.col-md-10').innerHTML = '<h3>Giỏ hàng trống !!!</h3>'
    document.querySelector('.col-md-2.p-4')?.remove();
    document.querySelector('.text-center')?.remove();
    // Tạo phần tử div với class "text-left" chứa nút quay về trang chủ
    const homeDiv = document.createElement('div');
    homeDiv.className = 'text-left';
    homeDiv.innerHTML = `
        <a class="btn btn-primary mt-5" href="/Home/Index">
            Quay về trang chủ
        </a>
    `;
    // Thêm phần tử nút quay về trang chủ vào trong cartContainer
    if (document.querySelector('.col-md-10')) {
        document.querySelector('.col-md-10').appendChild(homeDiv);
    }

}