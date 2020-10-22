import { SharedRoutingModule } from './shared-routing.module';

describe('SharedRoutingModule', () => {
  let sharedRoutingModule: SharedRoutingModule;

  beforeEach(() => {
    sharedRoutingModule = new SharedRoutingModule();
  });

  it('should create an instance', () => {
    expect(sharedRoutingModule).toBeTruthy();
  });
});
