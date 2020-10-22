import { ThirdPartyServicesModule } from './third-party-services.module';

describe('ThirdPartyServicesModule', () => {
  let thirdPartyServicesModule: ThirdPartyServicesModule;

  beforeEach(() => {
    thirdPartyServicesModule = new ThirdPartyServicesModule();
  });

  it('should create an instance', () => {
    expect(thirdPartyServicesModule).toBeTruthy();
  });
});
