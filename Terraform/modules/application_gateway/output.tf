output "application_gateway_id" {
  value = azurerm_application_gateway.application_gateway.id
}

output "ip_address" {
  value = azurerm_public_ip.public_ip.ip_address
}