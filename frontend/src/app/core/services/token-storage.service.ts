import { Injectable } from '@angular/core';
import { User } from '../models/user.model';

const ACCESS_TOKEN_KEY = 'coursehub_access_token';
const REFRESH_TOKEN_KEY = 'coursehub_refresh_token';
const USER_KEY = 'coursehub_user';

@Injectable({ providedIn: 'root' })
export class TokenStorageService {
  saveAccessToken(token: string): void {
    localStorage.setItem(ACCESS_TOKEN_KEY, token);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  }

  saveRefreshToken(token: string): void {
    localStorage.setItem(REFRESH_TOKEN_KEY, token);
  }

  saveUser(user: User): void {
    localStorage.setItem(USER_KEY, JSON.stringify(user));
  }

  getUser(): User | null {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) : null;
  }

  clear(): void {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  }
}
