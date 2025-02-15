﻿
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStoreMVC.Utility;

namespace BookShopMVC.Model
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string? OrderId { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        public ICollection<OrderItem> Items { get; } = new List<OrderItem>();
        public DateTime Date { get; set; }
        public Constants.OrderStatus Status { get; set; }
        public decimal Subtotal { get => Items.Sum(x => x.Price * x.Quantity); set => _ = value; }
        public decimal Vat { get => Subtotal - (Subtotal / (1 + Constants.Prices.Vat)); set => _ = value; }
        public decimal Shipping { get => Subtotal == 0 ? 0 : Constants.Prices.Shipping; set => _ = value; }
        public decimal Total { get => Subtotal + Shipping; set => _ = value; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [Display(Name = "Mã bưu điện")]
        public string? PostalCode { get; set; }
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }
    }
}
