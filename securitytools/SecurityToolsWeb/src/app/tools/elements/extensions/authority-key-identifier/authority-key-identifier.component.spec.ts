import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthorityKeyIdentifierComponent } from './authority-key-identifier.component';

describe('AuthorityKeyIdentifierComponent', () => {
  let component: AuthorityKeyIdentifierComponent;
  let fixture: ComponentFixture<AuthorityKeyIdentifierComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AuthorityKeyIdentifierComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AuthorityKeyIdentifierComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
