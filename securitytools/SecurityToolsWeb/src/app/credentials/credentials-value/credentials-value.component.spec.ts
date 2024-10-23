import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CredentialsValueComponent } from './credentials-value.component';

describe('ItemComponent', () => {
  let component: CredentialsValueComponent;
  let fixture: ComponentFixture<CredentialsValueComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CredentialsValueComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CredentialsValueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
