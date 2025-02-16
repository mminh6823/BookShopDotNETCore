﻿
shoppingCartBtns = document.querySelectorAll('.shopping-cart-btn')
const shoppingCartItemsAmount = document.querySelector('#cart-amount')

shoppingCartBtns.forEach(btn => {
    btn.addEventListener('click', () => {
        const productId = parseInt(btn.getAttribute('data-id'))
        const productQuantityInput = document.querySelector(`#quantity-${productId}`)
        const productQuantity = productQuantityInput ? parseInt(productQuantityInput.value) : 1
        const product = {
            ProductId: productId,
            Quantity: productQuantity
        }

        fetch(`/api/user/cart/`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(product)
        })
            .then(res => res.json())
            .then(data => {
                const toast = {
                    title: "Thành công",
                    message: "Đã thêm vào giỏ hàng.",
                    status: TOAST_STATUS.SUCCESS,
                    timeout: 2000
                }

                if (!data || data.status != "success") {
                    toast.title = "Lỗi"
                    toast.message = "Gặp vấn đề khi thêm vào giỏ hàng."
                    toast.status = TOAST_STATUS.DANGER
                }

                Toast.create(toast);
                refreshCartProductAmount(data.data.itemsQuantity)
            })
    })
})

function refreshCartProductAmount(cartQuantity) {
    if (cartQuantity != undefined) {
        shoppingCartItemsAmount.innerHTML = `(${cartQuantity})`
        return
    }
    fetch('/api/user/cart/', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(res => {
            if (!res.ok) {
                return;
            }
            return res.json()

        })
        .then(data => {
            if (data) {
                shoppingCartItemsAmount.innerHTML = `(${data.data.itemsQuantity})`
            }
        })
}

refreshCartProductAmount()