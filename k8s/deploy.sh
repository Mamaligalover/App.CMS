#!/bin/bash

echo "Deploying App CMS to Kubernetes..."

# Apply all manifests in order
kubectl apply -f 00-namespace.yaml
kubectl apply -f 01-secret.yaml
kubectl apply -f 02-configmap.yaml
kubectl apply -f 03-pvc.yaml
kubectl apply -f 04-postgres.yaml
kubectl apply -f 05-minio.yaml

# Wait for dependencies to be ready
echo "Waiting for PostgreSQL to be ready..."
kubectl wait --for=condition=available --timeout=300s deployment/postgres -n app-cms

echo "Waiting for MinIO to be ready..."
kubectl wait --for=condition=available --timeout=300s deployment/minio -n app-cms

# Deploy the application
kubectl apply -f 06-app-deployment.yaml
kubectl apply -f 07-app-service.yaml
kubectl apply -f 08-ingress.yaml

echo "Waiting for App CMS to be ready..."
kubectl wait --for=condition=available --timeout=300s deployment/app-cms -n app-cms

echo "Deployment complete!"
echo ""
echo "App CMS is available at: https://cms.cmv.md"
echo ""
echo "To check the status:"
echo "  kubectl get all -n app-cms"
echo ""
echo "To view logs:"
echo "  kubectl logs -f deployment/app-cms -n app-cms"