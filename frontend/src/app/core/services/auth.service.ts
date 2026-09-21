import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
} from '../models/auth.model';
import { User } from '../models/user.model';
import { TokenStorageService } from './token-storage.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(
    this.tokenStorage.getUser(),
  );
  currentUser$: Observable<User | null> =
    this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private tokenStorage: TokenStorageService,
  ) {}

  register(request: RegisterRequest): Observable<User> {
    return this.http.post<User>(`${environment.apiUrl}/auth/register`, request);
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/login`, request)
      .pipe(
        tap((response) => {
          this.tokenStorage.saveAccessToken(response.accessToken);
          this.tokenStorage.saveRefreshToken(response.refreshToken);
          this.tokenStorage.saveUser(response.user);
          this.currentUserSubject.next(response.user);
        }),
      );
  }

  logout(): void {
    this.tokenStorage.clear();
    this.currentUserSubject.next(null);
  }

  isLoggedIn(): boolean {
    return !!this.tokenStorage.getAccessToken();
  }

  getRole(): string | null {
    return this.tokenStorage.getUser()?.role ?? null;
  }
}
