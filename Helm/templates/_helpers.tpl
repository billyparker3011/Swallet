{{- define "common.labels" }}
app.kubernetes.io/name: {{ .Release.Name }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{- define "host" -}}
{{- $domain := .domain -}}
{{- printf "%s.%s" .Release.Name $domain -}}
{{- end -}}
