import { Routes } from '@angular/router';
import { LoginComponent } from './auth/pages/login/login.component';
import { EmptyLayoutComponent } from '../pages/empty-layout/empty-layout.component';
import { MainLayoutComponent as MainLayoutComponent } from '../pages/main-layout/main-layout.component';
import { MainPageComponent } from '../pages/main-page/main-page.component';
import { authGuard } from './auth/services/AuthGuard';
import { AddressTypeComponent } from './address-type/address-type.component';
import { CompanyRegisterLayoutComponent } from '../pages/company-register-layout/company-register-layout.component';
import { CompanyRegisterComponent } from './company/register/company-register.component';
import { CompanyEditComponent } from './company/edit/company-edit.component';
import { CompanyListComponent } from './company/list/company-list.component';
import { SocialMediaComponent } from './social-media/social-media.component';
import { EmailTypeComponent } from './email-type/email-type.component';
import { PhoneTypeComponent } from './phone-type/phone-type.component';
import { TitleComponent } from './title/title.component';
import { DocumentTypeComponent } from './document-type/document-type.component';

export const routes: Routes = [
  // Siempre redirigir a login inicialmente
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: '',
    component: EmptyLayoutComponent, // layout liviano para login
    children: [
      { path: 'login', component: LoginComponent }
    ]
  },
  {
    path: '',
    component: MainLayoutComponent, // layout completo para app logueada
    children: [
      { path: 'dashboard', component: MainPageComponent, canActivate: [authGuard] },
      // Mantener rutas de CRM pero con guards (pueden fallar por ahora)
      { path: 'phonetype', component: PhoneTypeComponent, canActivate: [authGuard] },
      { path: 'emailtype', component: EmailTypeComponent, canActivate: [authGuard]},
      { path: 'addresstype', component: AddressTypeComponent, canActivate: [authGuard]},
      { path: 'socialmedia', component: SocialMediaComponent, canActivate: [authGuard]},
      { path: 'title', component: TitleComponent, canActivate: [authGuard] },
      { path: 'documenttype', component: DocumentTypeComponent, canActivate: [authGuard] },
      { path: 'company/register', component: CompanyRegisterComponent },
      { path: 'company/list', component: CompanyListComponent, canActivate: [authGuard] },
      { path: 'company/edit/:id', component: CompanyEditComponent, canActivate: [authGuard] }
    ]
  },
  {
    path: '**',
    redirectTo: 'login',
    pathMatch: 'full'
  }
];