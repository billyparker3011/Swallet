######################################################
##                  Storage Account                 ##
######################################################
resource "azurerm_storage_account" "storage_account" {
  name                     = var.storage_account_name
  resource_group_name      = var.resource_group_name
  location                 = var.location
  account_tier             = var.account_tier
  account_replication_type = var.account_replication_type
  min_tls_version          = var.min_tls_version
  tags                     = var.tags

  blob_properties {
    cors_rule {
      allowed_origins    = var.allowed_origins
      allowed_methods    = var.allowed_methods
      allowed_headers    = var.allowed_headers
      exposed_headers    = var.exposed_headers
      max_age_in_seconds = var.max_age_in_seconds
    }

    delete_retention_policy {
      days = var.blob_properties_delete_retention_policy_days
    }

    container_delete_retention_policy {
      days = var.blob_properties_container_delete_retention_policy_days
    }

    versioning_enabled = var.blob_properties_versioning_enabled
  }
}

######################################################
##             Storage Account Container            ##
######################################################
resource "azurerm_storage_container" "container" {
  for_each              = toset(var.containers)
  name                  = each.value
  storage_account_name  = azurerm_storage_account.storage_account.name
  container_access_type = var.container_access_type
}

######################################################
##                   Storage Blob                   ##
######################################################
resource "azurerm_storage_blob" "storage_blob" {
  depends_on             = [azurerm_storage_container.container]
  for_each               = { for blob in var.blobs : blob.path => blob }
  name                   = each.value.path
  storage_account_name   = azurerm_storage_account.storage_account.name
  storage_container_name = each.value.container
  type                   = var.type
  source_uri             = "${var.source_uri}/${each.value.container}/${each.value.path}${var.SAS}"
  content_type           = each.value.content_type

  lifecycle {
    ignore_changes = [
      content_md5
    ]
  }
}