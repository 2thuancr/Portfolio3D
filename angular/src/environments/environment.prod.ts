import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44339/',
  redirectUri: baseUrl,
  clientId: 'Portfolio3D_App',
  responseType: 'code',
  scope: 'offline_access Portfolio3D',
  requireHttps: true,
};

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'Portfolio3D',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44339',
      rootNamespace: 'Portfolio3D',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
  remoteEnv: {
    url: '/getEnvConfig',
    mergeStrategy: 'deepmerge'
  }
} as Environment;
