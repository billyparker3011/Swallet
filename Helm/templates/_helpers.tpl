{{- define "common.labels" }}
app.kubernetes.io/name: {{ .Release.Name }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{- define "hosts" -}}
{{- $domains := (index .Values.applications .Values.applicationName).domains -}}
{{- if $domains -}}
{{- range $index, $domain := $domains }}
- {{ printf "%s.%s" .Release.Name $domain }}
{{- end }}
{{- else -}}
- {{ printf "%s.%s" .Release.Name (index .Values.applications .Values.applicationName).domain }}
{{- end -}}
{{- end -}}
