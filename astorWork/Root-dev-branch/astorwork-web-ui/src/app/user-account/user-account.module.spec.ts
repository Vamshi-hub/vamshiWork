import { UserAccountModule } from './user-account.module';

describe('UserAccountModule', () => {
  let userAccountModule: UserAccountModule;

  beforeEach(() => {
    userAccountModule = new UserAccountModule();
  });

  it('should create an instance', () => {
    expect(userAccountModule).toBeTruthy();
  });
});
